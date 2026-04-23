using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace TextboxControl
{
    /// <summary>
    /// Interpreter for textbox control sequences.
    /// Converts a source string into a revealed glyph buffer, style runs, and active animation regions.
    /// </summary>
    public class Reducer
    {
        private const int SkipSafetyBudget = 1000000;
        private const int TickSafetyBudget = 10000;

        private SequenceCursor _cursor;
        private bool _isPlaying;
        private bool _previewMode;

        private double _timeSincePlay;
        private double _timeAccumulator;
        private double _timeUntilNextAction;

        private float _typewriterIntervalSeconds;
        private bool _styleChanged;
        private CharBaseState _style;

        private readonly List<char> _buffer = new List<char>();
        private readonly List<StyleRun> _runs = new List<StyleRun>();

        private readonly List<Region> _regions = new List<Region>();
        private readonly List<(int offset, int length)> _paramSpans = new List<(int offset, int length)>(8);
        private int _nextRegionId;
        private int _regionVersion;

        /// <summary>
        /// Fired once when playback reaches the end of input.
        /// </summary>
        public event Action OnComplete;

        /// <summary>
        /// Fired when the input stream is malformed or a method argument is invalid.
        /// </summary>
        public event Action<string> OnError;

        /// <summary>
        /// True while the reducer is actively stepping through the source stream.
        /// </summary>
        public bool IsPlaying => _isPlaying;

        /// <summary>
        /// Number of plain glyphs emitted into <see cref="DisplayBuffer"/>.
        /// </summary>
        public int RevealedCount => _buffer.Count;

        /// <summary>
        /// Wall-clock playback time since the last <see cref="Play"/> call.
        /// </summary>
        public double TimeSincePlay => _timeSincePlay;

        /// <summary>
        /// Current plain character output (without TMP tags).
        /// </summary>
        public IReadOnlyList<char> DisplayBuffer => _buffer;

        /// <summary>
        /// Style segments that map one-to-one onto <see cref="DisplayBuffer"/>.
        /// </summary>
        public IReadOnlyList<StyleRun> StyleRuns => _runs;

        /// <summary>
        /// Active animation regions for already-emitted characters.
        /// </summary>
        public IReadOnlyList<Region> Regions => _regions;

        internal List<Region> RegionsDirect => _regions;
        internal int RegionVersion => _regionVersion;
        internal bool LogExternalControls { get; set; } = true;
        private bool IsPreview => _previewMode;

        /// <summary>
        /// Starts playback from the beginning of <paramref name="source"/>.
        /// </summary>
        /// <param name="source">Raw textbox stream with embedded control sequences.</param>
        /// <param name="previewMode">If true, timing and external controls are ignored.</param>
        public void Play(string source, bool previewMode = false)
        {
            _cursor = new SequenceCursor(source ?? string.Empty);
            _isPlaying = true;
            ResetState(previewMode, resetClock: true);
        }

        /// <summary>
        /// Stops playback and clears interpreted output.
        /// </summary>
        public void Stop()
        {
            _cursor = null;
            _isPlaying = false;
            ResetState(previewMode: false, resetClock: false);
        }

        /// <summary>
        /// Consumes the remaining input immediately, bypassing typewriter timing.
        /// </summary>
        public void Skip()
        {
            if (!_isPlaying)
            {
                return;
            }

            _typewriterIntervalSeconds = 0f;
            _timeUntilNextAction = 0d;

            // Safety guard prevents infinite loops on malformed control streams.
            int safety = SkipSafetyBudget;
            while (_isPlaying && safety-- > 0)
            {
                StepOnce();
            }
        }

        /// <summary>
        /// Advances playback using frame delta time.
        /// </summary>
        /// <param name="deltaTime">Elapsed seconds for this frame.</param>
        public void Tick(float deltaTime)
        {
            _timeSincePlay += deltaTime;

            if (!_isPlaying)
            {
                return;
            }

            _timeAccumulator += deltaTime;

            // Run until the next scheduled delay/typewriter gate.
            int safety = TickSafetyBudget;
            while (_isPlaying && _timeAccumulator >= _timeUntilNextAction && safety-- > 0)
            {
                _timeAccumulator -= _timeUntilNextAction;
                _timeUntilNextAction = 0d;
                StepOnce();
            }
        }

        private void ResetState(bool previewMode, bool resetClock)
        {
            if (resetClock)
            {
                _timeSincePlay = 0d;
            }

            _timeAccumulator = 0d;
            _timeUntilNextAction = 0d;
            _previewMode = previewMode;

            _typewriterIntervalSeconds = 0f;
            _style = CharBaseState.Default;
            _styleChanged = true;

            _buffer.Clear();
            _runs.Clear();

            _regions.Clear();
            _nextRegionId = 1;
            _regionVersion = 0;
        }

        private void StepOnce()
        {
            if (_cursor == null || _cursor.IsAtEnd)
            {
                CompleteIfPlaying();
                return;
            }

            StepResult result = _cursor.Step(out char glyph);
            switch (result)
            {
                case StepResult.Glyph:
                    EmitGlyph(glyph);
                    if (_typewriterIntervalSeconds > 0f)
                    {
                        _timeUntilNextAction += _typewriterIntervalSeconds;
                    }
                    break;

                case StepResult.SequenceStart:
                    Dispatch(_cursor.CurrentMethod);
                    break;

                case StepResult.End:
                    CompleteIfPlaying();
                    break;

                case StepResult.Error:
                    ReportError($"Malformed control sequence near index {_cursor.Index}.");
                    break;
            }
        }

        private void CompleteIfPlaying()
        {
            if (!_isPlaying)
            {
                return;
            }

            _isPlaying = false;
            OnComplete?.Invoke();
        }

        private void EndSequence()
        {
            if (!_cursor.EndSequence())
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
                    HandleStyleToggle(ref _style.Bold);
                    break;

                case MethodCode.Italic:
                    HandleStyleToggle(ref _style.Italic);
                    break;

                case MethodCode.Underline:
                    HandleStyleToggle(ref _style.Underline);
                    break;

                case MethodCode.Strikethrough:
                    HandleStyleToggle(ref _style.Strikethrough);
                    break;

                case MethodCode.Size:
                    HandleSize();
                    break;

                case MethodCode.CharSpacing:
                    HandleStyleFloat(ref _style.CharSpacing);
                    break;

                case MethodCode.BaselineOffset:
                    HandleStyleFloat(ref _style.BaselineOffset);
                    break;

                case MethodCode.Typewriter:
                    HandleTypewriter();
                    break;

                case MethodCode.Delay:
                    HandleDelay();
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

                case MethodCode.AnimClear:
                    HandleAnimClear();
                    break;

                case MethodCode.Mouth:
                case MethodCode.Eye:
                case MethodCode.Brow:
                case MethodCode.BodyPose:
                case MethodCode.Sound:
                default:
                    HandleExternalControl(method);
                    break;
            }
        }

        private void HandleResetAll()
        {
            EndSequence();
            _style = CharBaseState.Default;
            _styleChanged = true;
            ClearRegions();
        }

        private void HandleNewline()
        {
            EndSequence();
            EmitGlyph('\n');
        }

        private void HandleClearAll()
        {
            EndSequence();
            ClearRegions();
        }

        private void HandleStyleToggle(ref bool field)
        {
            bool value = true;
            if (!_cursor.TryReadBoolParam(out bool parsed, out string raw))
            {
                if (raw != null)
                {
                    ReportError($"Expected bool (1/0, on/off, true/false), got \"{raw}\".");
                }
            }
            else
            {
                value = parsed;
            }

            EndSequence();
            field = value;
            _styleChanged = true;
        }

        private void HandleStyleFloat(ref float field)
        {
            HandleFloat(ref field);
            _styleChanged = true;
        }

        private void HandleFloat(ref float field)
        {
            if (_cursor.TryReadParam(out string raw))
            {
                if (raw == "reset")
                {
                    field = 0f;
                }
                else if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                {
                    field = value;
                }
                else
                {
                    ReportError($"Expected number or 'reset', got \"{raw}\".");
                }
            }

            EndSequence();
        }

        private void HandleSize()
        {
            if (_cursor.TryReadParam(out string raw))
            {
                if (raw == "reset")
                {
                    _style.SizeOverride = float.NaN;
                }
                else if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                {
                    _style.SizeOverride = value;
                }
                else
                {
                    ReportError($"Size got unrecognized param \"{raw}\".");
                }
            }

            EndSequence();
            _styleChanged = true;
        }

        private void HandleTypewriter()
        {
            int ms = 0;
            if (!_cursor.TryReadIntParam(out ms, out string raw) && raw != null)
            {
                ReportError($"Typewriter expects int ms, got \"{raw}\".");
            }

            EndSequence();
            _typewriterIntervalSeconds = IsPreview || ms <= 0 ? 0f : ms / 1000f;
        }

        private void HandleDelay()
        {
            int ms = 0;
            if (!_cursor.TryReadIntParam(out ms, out string raw) && raw != null)
            {
                ReportError($"Delay expects int ms, got \"{raw}\".");
            }

            EndSequence();

            if (!IsPreview && ms > 0)
            {
                _timeUntilNextAction += ms / 1000f;
            }
        }

        private void HandleColor()
        {
            if (_cursor.TryReadParam(out string raw))
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
                    ReportError($"Color got unrecognized param \"{raw}\".");
                }
            }
            else
            {
                ReportError("Color missing param.");
            }

            EndSequence();
            _styleChanged = true;
        }

        private void HandleFont()
        {
            if (_cursor.TryReadParam(out string raw))
            {
                _style.FontName = raw == "reset" ? null : raw;
            }
            else
            {
                ReportError("Font missing param.");
            }

            EndSequence();
            _styleChanged = true;
        }

        private void HandleAnimClear()
        {
            if (!_cursor.TryReadParam(out string raw))
            {
                ReportError("AnimClear needs id or name.");
                EndSequence();
                return;
            }

            if (IsPreview)
            {
                DrainRemainingParams();
                EndSequence();
                return;
            }

            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
            {
                RemoveRegionById(id);
            }
            else
            {
                RemoveRegionsByName(raw);
            }

            EndSequence();
        }

        private void HandleAnimStart()
        {
            int start = _buffer.Count;

            if (!_cursor.TryReadIntParam(out int length, out string rawLength))
            {
                if (rawLength == null)
                {
                    ReportError("AnimStart: length must be int.");
                }
                else
                {
                    ReportError($"AnimStart: length must be int, got \"{rawLength}\".");
                }

                EndSequence();
                return;
            }

            if (length <= 0)
            {
                ReportError("Anim region length must be > 0.");
                EndSequence();
                return;
            }

            if (!_cursor.TryReadParam(out string animName))
            {
                ReportError("Anim region missing animation name.");
                EndSequence();
                return;
            }

            if (IsPreview)
            {
                DrainRemainingParams();
                EndSequence();
                return;
            }

            CaptureRemainingParams(_paramSpans);
            EndSequence();

            string source = _cursor.Source;
            // Optional trailing identifier token becomes the region name.
            string regionName = TryTakeRegionName(source, _paramSpans);

            Animation.IAnimation animation = Animation.AnimationRegistry.Create(animName, source, _paramSpans);

            _regions.Add(new Region
            {
                Id = _nextRegionId++,
                Name = regionName,
                Start = start,
                Length = length,
                StartTime = _timeSincePlay,
                Animation = animation,
            });
            _regionVersion++;

            if (animation == null)
            {
                ReportError($"Unknown animation \"{animName}\"");
            }
        }

        private void HandleExternalControl(int method)
        {
            if (IsPreview || !LogExternalControls)
            {
                DrainRemainingParams();
                EndSequence();
                return;
            }

            CaptureRemainingParams(_paramSpans);
            EndSequence();

            if (_paramSpans.Count == 0)
            {
                Debug.Log($"[TextboxControl] external control ignored: {method}");
                return;
            }

            string source = _cursor.Source;
            string[] parameters = new string[_paramSpans.Count];
            for (int i = 0; i < _paramSpans.Count; i++)
            {
                (int offset, int length) = _paramSpans[i];
                parameters[i] = source.Substring(offset, length);
            }

            Debug.Log($"[TextboxControl] external control ignored: {method}: {string.Join(", ", parameters)}");
        }

        private void ClearRegions()
        {
            if (IsPreview)
            {
                return;
            }

            _regions.Clear();
            _regionVersion++;
        }

        private void RemoveRegionById(int id)
        {
            for (int i = _regions.Count - 1; i >= 0; i--)
            {
                if (_regions[i].Id != id)
                {
                    continue;
                }

                _regions.RemoveAt(i);
                _regionVersion++;
                return;
            }
        }

        private void RemoveRegionsByName(string name)
        {
            bool removed = false;
            for (int i = _regions.Count - 1; i >= 0; i--)
            {
                if (_regions[i].Name != name)
                {
                    continue;
                }

                _regions.RemoveAt(i);
                removed = true;
            }

            if (removed)
            {
                _regionVersion++;
            }
        }

        private void DrainRemainingParams()
        {
            while (_cursor.TryReadParamSpan(out _, out _))
            {
            }
        }

        private void CaptureRemainingParams(List<(int offset, int length)> destination)
        {
            destination.Clear();
            while (_cursor.TryReadParamSpan(out int offset, out int length))
            {
                destination.Add((offset, length));
            }
        }

        private static string TryTakeRegionName(string source, List<(int offset, int length)> spans)
        {
            if (spans.Count == 0)
            {
                return null;
            }

            (int offset, int length) = spans[spans.Count - 1];
            if (!IsValidIdentSpan(source, offset, length))
            {
                return null;
            }

            spans.RemoveAt(spans.Count - 1);
            return source.Substring(offset, length);
        }

        private static bool IsValidIdentSpan(string src, int offset, int length)
        {
            if (length == 0)
            {
                return false;
            }

            int end = offset + length;
            for (int i = offset; i < end; i++)
            {
                char c = src[i];
                if (c == '=' || c == ',')
                {
                    return false;
                }

                if (i == offset)
                {
                    if (!(char.IsLetter(c) || c == '_'))
                    {
                        return false;
                    }
                }
                else if (!(char.IsLetterOrDigit(c) || c == '_'))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryParseHexColor(string s, out Color color)
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

            color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
            return true;
        }

        private static bool IsHexDigit(char c)
        {
            return (c >= '0' && c <= '9') ||
                   (c >= 'a' && c <= 'f') ||
                   (c >= 'A' && c <= 'F');
        }

        private static int HexVal(char c)
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

        private void ReportError(string msg)
        {
            if (OnError != null)
            {
                OnError(msg);
                return;
            }

            Debug.LogWarning("[TextboxControl] " + msg);
        }
    }
}
