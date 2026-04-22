namespace TextboxControl
{
    public enum StepResult
    {
        Glyph,
        ControlStart,
        End,
        Error,
    }

    public class ControlCursor
    {
        private const char ESC = '\x1b';
        public const char TERM = '\x1c';

        private readonly string _src;
        private int _index;
        private int _currentMethod;

        public ControlCursor(string source)
        {
            _src = source ?? "";
            _index = 0;
        }

        public int Index => _index;
        public bool IsAtEnd => _index >= _src.Length;
        public string Source => _src;
        public int CurrentMethod => _currentMethod;

        public StepResult Step(out char glyph)
        {
            glyph = '\0';

            if (_index >= _src.Length)
            {
                return StepResult.End;
            }

            if (_src[_index] != ESC)
            {
                glyph = _src[_index++];
                return StepResult.Glyph;
            }

            return TryReadHeader() ? StepResult.ControlStart : StepResult.Error;
        }

        public bool ReadNumericOrHexParam(out string token)
        {
            return ReadParam(out token);
        }

        public bool ReadStringParam(out string token)
        {
            return ReadParam(out token);
        }

        public bool ReadParamSpan(out int offset, out int length)
        {
            return TryReadParamBounds(out offset, out length);
        }

        public bool EndControl()
        {
            if (_index < _src.Length && _src[_index] == TERM)
            {
                _index++;
                return true;
            }

            return false;
        }

        private bool TryReadHeader()
        {
            int start = _index;

            if (_index + 1 >= _src.Length || _src[_index + 1] != '[')
            {
                _index = start + 1;
                return false;
            }

            int i = _index + 2;
            int methodStart = i;

            while (i < _src.Length && IsDigit(_src[i]))
            {
                i++;
            }

            if (i == methodStart || !int.TryParse(_src.Substring(methodStart, i - methodStart), out _currentMethod))
            {
                _index = start + 1;
                return false;
            }

            _index = i;
            return true;
        }

        private bool ReadParam(out string token)
        {
            token = null;

            if (!TryReadParamBounds(out int offset, out int length))
            {
                return false;
            }

            token = _src.Substring(offset, length);
            return true;
        }

        private bool TryReadParamBounds(out int offset, out int length)
        {
            offset = 0;
            length = 0;

            if (_index >= _src.Length || _src[_index] == TERM)
            {
                return false;
            }

            if (_src[_index] != ';')
            {
                return false;
            }

            _index++;
            offset = _index;

            while (_index < _src.Length)
            {
                char c = _src[_index];
                if (c == ';' || c == TERM)
                {
                    length = _index - offset;
                    return true;
                }

                _index++;
            }

            return false;
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
    }
}
