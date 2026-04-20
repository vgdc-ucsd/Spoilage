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

        private float _typewriterIntervalSeconds = 0f;
        private bool _styleChanged = true;

        private readonly List<char> _buffer = new List<char>();
        private readonly List<StyleRun> _runs = new List<StyleRun>();

        private CharBaseState _style = CharBaseState.Default;


        private readonly List<Region> _regions = new List<Region>();
        private int _nextRegionId = 1;
        private int _regionVersion;

        public event Action OnComplete;
        public event Action<string> OnError;

        public bool IsPlaying => _isPlaying;
        public int RevealedCount => _buffer.Count;
        public double TimeSincePlay => _timeSincePlay;
        public IReadOnlyList<char> DisplayBuffer => _buffer;
        public IReadOnlyList<StyleRun> StyleRuns => _runs;
        public IReadOnlyList<Region> Regions => _regions;

        internal List<Region> RegionsDirect => _regions;
        internal int RegionVersion => _regionVersion;
        internal bool LogExternalControls { get; set; } = true;

        public void Play(string source)
        {
            _cursor = new ControlCursor(source ?? "");
            _isPlaying = true;
            _timeSincePlay = 0;
            _timeAccumulator = 0;
            _timeUntilNextAction = 0;
            _typewriterIntervalSeconds = 0f;
            _style = CharBaseState.Default;
            _styleChanged = true;
            _buffer.Clear();
            _runs.Clear();
            _regions.Clear();
            _nextRegionId = 1;
            _regionVersion = 0;
        }

        public void Stop()
        {
            _cursor = null;
            _isPlaying = false;
            _timeAccumulator = 0;
            _timeUntilNextAction = 0;
            _typewriterIntervalSeconds = 0f;
            _style = CharBaseState.Default;
            _styleChanged = true;
            _buffer.Clear();
            _runs.Clear();
            _regions.Clear();
            _nextRegionId = 1;
            _regionVersion = 0;
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
                if (_isPlaying)
                {
                    _isPlaying = false; OnComplete?.Invoke();
                }
                return;
            }

            StepResult result = _cursor.Step(out char glyph);
            switch (result)
            {
                case StepResult.Glyph:
                    EmitGlyph(glyph);
                    if (_typewriterIntervalSeconds > 0f)
                        _timeUntilNextAction += _typewriterIntervalSeconds;
                    break;

                case StepResult.ControlStart:
                    Dispatch(_cursor.CurrentMethod);
                    break;

                case StepResult.End:
                    _isPlaying = false;
                    OnComplete?.Invoke();
                    break;

                case StepResult.Error:
                    ReportError($"Malformed control sequence near index {_cursor.Index}.");
                    break;
            }
        }

        private void EndControl()
        {
            if (!_cursor.EndControl())
            {
                ReportError($"Malformed control sequence (bad terminator) near index {_cursor.Index}.");
            }
        }

        private void EmitGlyph(char c)
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

        private void Dispatch(int method)
        {
            switch ((MethodCode)method)
            {
                case MethodCode.ResetAll:
                    HandleResetAll();
                    break;
                case MethodCode.Newline:
                    HandleNewline();
                    break;
                case MethodCode.AnimClearAll:
                    HandleClearAll();
                    break;

                case MethodCode.Bold:
                    HandleBool(ref _style.Bold);
                    _styleChanged = true;
                    break;
                case MethodCode.Italic:
                    HandleBool(ref _style.Italic);
                    _styleChanged = true; break;
                case MethodCode.Underline:
                    HandleBool(ref _style.Underline);
                    _styleChanged = true;
                    break;
                case MethodCode.Strikethrough:
                    HandleBool(ref _style.Strikethrough);
                    _styleChanged = true;
                    break;
                case MethodCode.Size:
                    HandleSize();
                    break;
                case MethodCode.CharSpacing:
                    HandleFloat(ref _style.CharSpacing);
                    _styleChanged = true;
                    break;
                case MethodCode.BaselineOffset:
                    HandleFloat(ref _style.BaselineOffset);
                    _styleChanged = true;
                    break;
                case MethodCode.Typewriter:
                    HandleTypewriter();
                    break;
                case MethodCode.Delay:
                    HandleDelay();
                    break;
                case MethodCode.AnimClear:
                    HandleAnimClear();
                    break;

                case MethodCode.Color:
                    HandleColor();
                    break;
                case MethodCode.Font:
                    HandleFont();
                    break;

                case MethodCode.AnimStart:
                    HandleAnimStart();
                    break;

                case MethodCode.Mouth:
                case MethodCode.Eye:
                case MethodCode.Brow:
                case MethodCode.BodyPose:
                case MethodCode.Sound:
                    HandleExternalControl(method);
                    break;
                default:
                    HandleExternalControl(method);
                    break;
            }
        }

        private void HandleResetAll()
        {
            EndControl();
            _style = CharBaseState.Default;
            _styleChanged = true;
            _regions.Clear();
            _regionVersion++;
        }

        private void HandleNewline()
        {
            EndControl();
            EmitGlyph('\n');
        }

        void HandleClearAll()
        {
            EndControl();
            _regions.Clear();
            _regionVersion++;
        }


        private void HandleBool(ref bool field)
        {
            bool value = true;
            if (_cursor.ReadNumericOrHexParam(out string raw))
            {
                value = raw == "1" || raw == "on" || raw == "true";
            }

            EndControl();
            field = value;
        }

        private void HandleSize()
        {
            if (_cursor.ReadNumericOrHexParam(out string raw))
            {
                if (raw == "reset")
                {
                    _style.SizeOverride = float.NaN;
                }
                else if (float.TryParse(raw, System.Globalization.NumberStyles.Float,
                                        System.Globalization.CultureInfo.InvariantCulture, out float value))
                {
                    _style.SizeOverride = value;
                }
                else
                {
                    ReportError($"Size got unrecognized param \"{raw}\".");
                }
            }

            EndControl();
            _styleChanged = true;
        }

        private void HandleFloat(ref float field)
        {
            if (_cursor.ReadNumericOrHexParam(out string raw))
            {
                if (raw == "reset")
                {
                    field = 0f;
                }
                else if (float.TryParse(raw, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float value))
                {
                    field = value;
                }
                else
                {
                    ReportError($"Expected number or 'reset', got \"{raw}\".");
                }
            }

            EndControl();
        }

        private void HandleTypewriter()
        {
            int ms = 0;
            if (_cursor.ReadNumericOrHexParam(out string raw) && !int.TryParse(raw, out ms))
            {
                ReportError($"Typewriter expects int ms, got \"{raw}\".");
            }

            EndControl();
            _typewriterIntervalSeconds = ms <= 0 ? 0f : ms / 1000f;
        }

        private void HandleDelay()
        {
            int ms = 0;
            if (_cursor.ReadNumericOrHexParam(out string raw) && !int.TryParse(raw, out ms))
            {
                ReportError($"Delay expects int ms, got \"{raw}\".");
            }
            EndControl();
            if (ms > 0)
            {
                _timeUntilNextAction += ms / 1000f;
            }
        }

        private void HandleColor()
        {
            if (_cursor.ReadNumericOrHexParam(out string raw))
            {
                if (raw == "reset")
                {
                    _style.HasColor = false;
                    _style.Color = default;
                }
                else if (TryParseHexColor(raw, out Color c))
                {
                    _style.HasColor = true;
                    _style.Color = c;
                }
                else
                {
                    ReportError($"Color got unrecognized param \"{raw}\".");
                }
            }
            else
            {
                ReportError("Color missing param.");
            }
            EndControl();
            _styleChanged = true;
        }

        private void HandleFont()
        {
            if (_cursor.ReadStringParam(out string raw))
            {
                if (raw == "reset")
                {
                    _style.FontName = null;
                }
                else
                {
                    _style.FontName = raw;
                }
            }
            else
            {
                ReportError("Font missing param.");
            }
            EndControl();
            _styleChanged = true;
        }

        private void HandleAnimClear()
        {
            if (_cursor.ReadStringParam(out string raw))
            {
                if (int.TryParse(raw, out int id))
                {
                    for (int i = _regions.Count - 1; i >= 0; i--)
                    {
                        if (_regions[i].Id == id)
                        {
                            _regions.RemoveAt(i);
                            _regionVersion++; break;
                        }
                    }
                }
                else
                {
                    for (int i = _regions.Count - 1; i >= 0; i--)
                    {
                        if (_regions[i].Name == raw)
                        {
                            _regions.RemoveAt(i);
                            _regionVersion++;
                        }
                    }
                }
            }
            else
            {
                ReportError("AnimClear needs id or name.");
            }
            EndControl();
        }

        private void HandleAnimStart()
        {
            int start = _buffer.Count;
            if (!_cursor.ReadNumericOrHexParam(out string sLen) || !int.TryParse(sLen, out int length))
            {
                ReportError("AnimStart: length must be int.");
                EndControl(); return;
            }
            if (length <= 0)
            {
                ReportError("Anim region length must be > 0.");
                EndControl();
                return;
            }

            if (!_cursor.ReadStringParam(out string animName))
            {
                ReportError("Anim region missing animation name.");
                EndControl();
                return;
            }


            List<(int offset, int length)> spans = new List<(int offset, int length)>();
            while (_cursor.ReadParamSpan(out int o, out int l))
            {
                spans.Add((o, l));
            }
            EndControl();

            string regionName = null;
            if (spans.Count > 0)
            {
                (int lo, int ll) = spans[spans.Count - 1];
                if (IsValidIdentSpan(_cursor.Source, lo, ll))
                {
                    regionName = _cursor.Source.Substring(lo, ll);
                    spans.RemoveAt(spans.Count - 1);
                }
            }

            Animation.IAnimation anim = Animation.AnimationRegistry.Create(animName, _cursor.Source, spans);
            _regions.Add(new Region
            {
                Id = _nextRegionId++,
                Name = regionName,
                Start = start,
                Length = length,
                StartTime = _timeSincePlay,
                Animation = anim,
            });
            _regionVersion++;

            if (anim == null)
            {
                ReportError($"Unknown animation \"{animName}\"");
            }
        }

        private void HandleExternalControl(int method)
        {
            List<string> parms = new List<string>();
            while (_cursor.ReadStringParam(out string p))
            {
                parms.Add(p);
            }
            EndControl();

            if (!LogExternalControls)
            {
                return;
            }

            if (parms.Count == 0)
            {
                Debug.Log($"[TextboxControl] external control ignored: {method}");
            }
            else
            {
                Debug.Log($"[TextboxControl] external control ignored: {method}: {string.Join(", ", parms)}");
            }
        }

        private static bool IsValidIdentSpan(string src, int offset, int length)
        {
            if (length == 0) return false;
            for (int i = offset; i < offset + length; i++)
            {
                char c = src[i];
                if (c == '=' || c == ',') return false;
                if (i == offset && !(char.IsLetter(c) || c == '_')) return false;
                if (i > offset && !(char.IsLetterOrDigit(c) || c == '_')) return false;
            }
            return true;
        }

        static bool TryParseHexColor(string s, out Color color)
        {
            color = default;
            if (string.IsNullOrEmpty(s)) return false;
            int len = s.Length;
            if (len != 3 && len != 4 && len != 6 && len != 8) return false;
            for (int i = 0; i < len; i++) if (!IsHexDigit(s[i])) return false;

            byte r, g, b, a = 255;
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
            color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
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

        void ReportError(string msg)
        {
            if (OnError != null)
            {
                OnError(msg);
            }
            else
            {
                Debug.LogWarning("[TextboxControl] " + msg);
            }
        }
    }
}
