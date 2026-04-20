namespace TextboxControl{
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
        }

        public int Index => _index;
        public bool IsAtEnd => _index >= _src.Length;
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

        bool TryReadHeader()
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

        public bool ReadNumericOrHexParam(out string token)
        {
            token = null;

            if (_index >= _src.Length || _src[_index] == TERM)
            {
                return false;
            }

            if (_src[_index] != ';')
            {
                return false;
            }

            _index++;
            int start = _index;

            while (_index < _src.Length && _src[_index] != ';' && _src[_index] != TERM)
            {
                _index++;
            }

            if (_index >= _src.Length)
            {
                return false;
            }

            token = _src.Substring(start, _index - start);
            return true;
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

        static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
    }
}
