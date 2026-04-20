using System;
using System.Collections.Generic;

namespace TextboxControl
{
    public class Reducer
    {
        ControlCursor _cursor;
        private bool _isPlaying;
        private double _timeSincePlay;
        private double _timeAccumulator;
        private double _timeUntilNextAction;
        private float _typewriterIntervalSeconds;

        private readonly List<char> _buffer = new List<char>();

        public event Action OnComplete;
        public event Action<string> OnError;

        public bool IsPlaying => _isPlaying;
        public int RevealedCount => _buffer.Count;
        public double TimeSincePlay => _timeSincePlay;
        public IReadOnlyList<char> DisplayBuffer => _buffer;

        public void Play(string source)
        {
            _cursor = new ControlCursor(source ?? "");
            _isPlaying = true;
            _timeSincePlay = 0;
            _timeAccumulator = 0;
            _timeUntilNextAction = 0;
            _typewriterIntervalSeconds = 0;
            _buffer.Clear();
        }

        public void Stop()
        {
            _cursor = null;
            _isPlaying = false;
            _timeAccumulator = 0;
            _timeUntilNextAction = 0;
            _typewriterIntervalSeconds = 0;
            _buffer.Clear();
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
                    _buffer.Add(glyph);
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

        void Dispatch(int method)
        {
            switch ((MethodCode)method)
            {
                case MethodCode.ResetAll:
                    EndControl();
                    _typewriterIntervalSeconds = 0;
                    break;

                case MethodCode.Newline:
                    EndControl();
                    _buffer.Add('\n');
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

        void HandleTypewriter()
        {
            int ms = 0;
            if (_cursor.ReadNumericOrHexParam(out string raw) && !int.TryParse(raw, out ms))
                OnError?.Invoke($"Typewriter expects int ms, got {raw}.");

            EndControl();
            _typewriterIntervalSeconds = ms <= 0 ? 0 : ms / 1000f;
        }

        void HandleDelay()
        {
            int ms = 0;
            if (_cursor.ReadNumericOrHexParam(out string raw) && !int.TryParse(raw, out ms))
                OnError?.Invoke($"Delay expects int ms, got {raw}.");

            EndControl();
            if (ms > 0)
                _timeUntilNextAction += ms / 1000f;
        }

        void EndControl()
        {
            if (!_cursor.EndControl())
                OnError?.Invoke($"Malformed control terminator near index {_cursor.Index}.");
        }

        void Complete()
        {
            if (!_isPlaying)
                return;

            _isPlaying = false;
            OnComplete?.Invoke();
        }
    }
}
