using System;
using System.Collections.Generic;
using UnityEngine;

namespace TextboxControl
{
    public class Reducer
    {
        private ControlCursor _cursor;
        private bool _isPlaying;
        private double _timeSincePlay;
        private double _timeAccumulator;
        private double _timeUntilNextAction;
        private float _typewriterIntervalSeconds;
        private bool _styleChanged = true;
        private CharBaseState _style = CharBaseState.Default;

        private readonly List<char> _buffer = new List<char>();
        private readonly List<StyleRun> _runs = new List<StyleRun>();

        public event Action OnComplete;
        public event Action<string> OnError;

        public bool IsPlaying => _isPlaying;
        public int RevealedCount => _buffer.Count;
        public double TimeSincePlay => _timeSincePlay;
        public IReadOnlyList<char> DisplayBuffer => _buffer;
        public IReadOnlyList<StyleRun> StyleRuns => _runs;

        public void Play(string source)
        {
            _cursor = new ControlCursor(source ?? "");
            _isPlaying = true;
            _timeSincePlay = 0;
            _timeAccumulator = 0;
            _timeUntilNextAction = 0;
            _typewriterIntervalSeconds = 0;
            _style = CharBaseState.Default;
            _styleChanged = true;
            _buffer.Clear();
            _runs.Clear();
        }

        public void Stop()
        {
            _cursor = null;
            _isPlaying = false;
            _timeAccumulator = 0;
            _timeUntilNextAction = 0;
            _typewriterIntervalSeconds = 0;
            _style = CharBaseState.Default;
            _styleChanged = true;
            _buffer.Clear();
            _runs.Clear();
        }

        public void Skip()
        {
            if (!_isPlaying)
            {
                return;
            }

            _typewriterIntervalSeconds = 0;
            _timeUntilNextAction = 0;

            int safety = 1000000;
            while (_isPlaying && safety-- > 0)
            {
                StepOnce();
            }
        }

        public void Tick(float deltaTime)
        {
            _timeSincePlay += deltaTime;

            if (!_isPlaying)
            {
                return;
            }

            _timeAccumulator += deltaTime;

            int safety = 10000;
            while (_isPlaying && _timeAccumulator >= _timeUntilNextAction && safety-- > 0)
            {
                _timeAccumulator -= _timeUntilNextAction;
                _timeUntilNextAction = 0;
                StepOnce();
            }
        }

        void StepOnce()
        {
            if (_cursor == null || _cursor.IsAtEnd)
            {
                Complete();
                return;
            }

            StepResult result = _cursor.Step(out char glyph);
            switch (result)
            {
                case StepResult.Glyph:
                    EmitGlyph(glyph);
                    if (_typewriterIntervalSeconds > 0)
                        _timeUntilNextAction += _typewriterIntervalSeconds;
                    break;

                case StepResult.ControlStart:
                    Dispatch(_cursor.CurrentMethod);
                    break;

                case StepResult.End:
                    Complete();
                    break;

                case StepResult.Error:
                    OnError?.Invoke($"Malformed control sequence near index {_cursor.Index}.");
                    break;
            }
        }

        void EmitGlyph(char c)
        {
            _buffer.Add(c);

            if (!_styleChanged && _runs.Count > 0)
            {
                StyleRun last = _runs[_runs.Count - 1];
                last.Count++;
                _runs[_runs.Count - 1] = last;
                return;
            }

            _runs.Add(new StyleRun { Count = 1, Style = _style });
            _styleChanged = false;
        }

        void Dispatch(int method)
        {
            switch ((MethodCode)method)
            {
                case MethodCode.ResetAll:
                    EndControl();
                    _style = CharBaseState.Default;
                    _styleChanged = true;
                    _typewriterIntervalSeconds = 0;
                    break;
                case MethodCode.Newline:
                    EndControl();
                    EmitGlyph('\n');
                    break;
                case MethodCode.Bold:
                    HandleBool(ref _style.Bold);
                    break;
                case MethodCode.Italic:
                    HandleBool(ref _style.Italic);
                    break;
                case MethodCode.Underline:
                    HandleBool(ref _style.Underline);
                    break;
                case MethodCode.Strikethrough:
                    HandleBool(ref _style.Strikethrough);
                    break;
                case MethodCode.Color:
                    HandleColor();
                    break;
                case MethodCode.Font:
                    HandleFont();
                    break;
                case MethodCode.Size:
                    HandleSize();
                    break;
                case MethodCode.CharSpacing:
                    HandleFloat(ref _style.CharSpacing);
                    break;
                case MethodCode.BaselineOffset:
                    HandleFloat(ref _style.BaselineOffset);
                    break;
                case MethodCode.Typewriter:
                    HandleTypewriter();
                    break;
                case MethodCode.Delay:
                    HandleDelay();
                    break;
                default:
                    OnError?.Invoke($"Unsupported method {method}.");
                    while (_cursor.ReadNumericOrHexParam(out _)) { }
                    EndControl();
                    break;
            }
        }

        void HandleBool(ref bool field)
        {
            bool value = true;
            if (_cursor.ReadNumericOrHexParam(out string raw))
            {
                value = raw == "1" || raw == "on" || raw == "true";
            }

            EndControl();
            field = value;
            _styleChanged = true;
        }

        void HandleColor()
        {
            if (_cursor.ReadNumericOrHexParam(out string raw))
            {
                if (raw == "reset")
                {
                    _style.HasColor = false;
                    _style.Color = default;
                }
                else if (TryParseHexColor(raw, out Color color))
                {
                    _style.HasColor = true;
                    _style.Color = color;
                }
                else
                {
                    OnError?.Invoke($"Color got unrecognized param {raw}.");
                }
            }

            EndControl();
            _styleChanged = true;
        }

        void HandleFont()
        {
            if (_cursor.ReadNumericOrHexParam(out string raw))
            {
                _style.FontName = raw == "reset" ? null : raw;
            }

            EndControl();
            _styleChanged = true;
        }

        void HandleSize()
        {
            if (_cursor.ReadNumericOrHexParam(out string raw))
            {
                if (raw == "reset")
                {
                    _style.SizeOverride = float.NaN;
                }
                else if (float.TryParse(raw, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float value))
                {
                    _style.SizeOverride = value;
                }
                else
                {
                    OnError?.Invoke($"Size got unrecognized param {raw}.");
                }
            }

            EndControl();
            _styleChanged = true;
        }

        void HandleFloat(ref float field)
        {
            if (_cursor.ReadNumericOrHexParam(out string raw))
            {
                if (raw == "reset")
                {
                    field = 0;
                }
                else if (float.TryParse(raw, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float value))
                {
                    field = value;
                }
                else
                {
                    OnError?.Invoke($"Expected number or reset, got {raw}.");
                }
            }

            EndControl();
            _styleChanged = true;
        }

        void HandleTypewriter()
        {
            int ms = 0;
            if (_cursor.ReadNumericOrHexParam(out string raw) && !int.TryParse(raw, out ms))
            {
                OnError?.Invoke($"Typewriter expects int ms, got {raw}.");
            }

            EndControl();
            _typewriterIntervalSeconds = ms <= 0 ? 0 : ms / 1000f;
        }

        void HandleDelay()
        {
            int ms = 0;
            if (_cursor.ReadNumericOrHexParam(out string raw) && !int.TryParse(raw, out ms))
            {
                OnError?.Invoke($"Delay expects int ms, got {raw}.");
            }

            EndControl();
            if (ms > 0)
                _timeUntilNextAction += ms / 1000f;
        }

        void EndControl()
        {
            if (!_cursor.EndControl())
            {
                OnError?.Invoke($"Malformed control terminator near index {_cursor.Index}.");
            }
        }

        void Complete()
        {
            if (!_isPlaying)
            {
                return;
            }

            _isPlaying = false;
            OnComplete?.Invoke();
        }

        static bool TryParseHexColor(string s, out Color color)
        {
            color = default;
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }

            int len = s.Length;
            if (len != 3 && len != 4 && len != 6 && len != 8)
            {
                return false;
            }

            for (int i = 0; i < len; i++)
            {
                if (!IsHexDigit(s[i]))
                {
                    return false;
                }
            }

            byte r;
            byte g;
            byte b;
            byte a = 255;

            if (len <= 4)
            {
                r = (byte)(HexVal(s[0]) * 17);
                g = (byte)(HexVal(s[1]) * 17);
                b = (byte)(HexVal(s[2]) * 17);
                if (len == 4)
                {
                    a = (byte)(HexVal(s[3]) * 17);
                }
            }
            else
            {
                r = (byte)((HexVal(s[0]) << 4) | HexVal(s[1]));
                g = (byte)((HexVal(s[2]) << 4) | HexVal(s[3]));
                b = (byte)((HexVal(s[4]) << 4) | HexVal(s[5]));
                if (len == 8)
                {
                    a = (byte)((HexVal(s[6]) << 4) | HexVal(s[7]));
                }
            }

            color = new Color32(r, g, b, a);
            return true;
        }

        static bool IsHexDigit(char c)
        {
            return c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F';
        }

        static int HexVal(char c)
        {
            if (c <= '9')
            {
                return c - '0';
            }
            if (c <= 'F')
            {
                return c - 'A' + 10;
            }
            return c - 'a' + 10;
        }
    }
}
