using System;
using System.Globalization;

namespace TextboxControl
{
    /// <summary>
    /// Result of consuming one token from the raw control stream.
    /// </summary>
    public enum StepResult
    {
        Glyph,
        SequenceStart,
        End,
        Error,
    }

    /// <summary>
    /// Incremental parser over the textbox control stream.
    /// Reads and understands `ESC[method;param;...TERM` segments in text.
    /// </summary>
    public class SequenceCursor
    {
        private const char ESC = '\x1b';
        public const char TERM = '\x1c';

        private readonly string _src;
        private int _index;
        private int _currentMethod;

        public SequenceCursor(string source)
        {
            _src = source ?? string.Empty;
            _index = 0;
        }

        public int Index => _index;
        public bool IsAtEnd => _index >= _src.Length;
        public string Source => _src;
        public int CurrentMethod => _currentMethod;

        /// <summary>
        /// Consumes one token. Produces either a plain glyph or a control-sequence header (method code).
        /// </summary>
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

            return TryReadHeader() ? StepResult.SequenceStart : StepResult.Error;
        }

        /// <summary>
        /// Reads the next `;param` token as an unescaped string.
        /// </summary>
        public bool TryReadParam(out string token)
        {
            token = null;

            if (!TryReadParamBounds(out int offset, out int length))
            {
                return false;
            }

            token = _src.Substring(offset, length);
            return true;
        }

        /// <summary>
        /// Reads the next param token and parses it as an integer.
        /// </summary>
        public bool TryReadIntParam(out int value, out string raw)
        {
            value = 0;
            raw = null;
            if (!TryReadParam(out raw))
            {
                return false;
            }

            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Reads the next param token and parses it as a float.
        /// </summary>
        public bool TryReadFloatParam(out float value, out string raw)
        {
            value = 0f;
            raw = null;
            if (!TryReadParam(out raw))
            {
                return false;
            }

            return float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Reads the next param token and parses it as a boolean.
        /// </summary>
        public bool TryReadBoolParam(out bool value, out string raw)
        {
            value = false;
            raw = null;
            if (!TryReadParam(out raw))
            {
                return false;
            }

            if (raw == "1" ||
                raw.Equals("on", StringComparison.OrdinalIgnoreCase) ||
                raw.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                value = true;
                return true;
            }

            if (raw == "0" ||
                raw.Equals("off", StringComparison.OrdinalIgnoreCase) ||
                raw.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                value = false;
                return true;
            }

            return false;
        }

        public bool TryReadParamSpan(out int offset, out int length)
        {
            return TryReadParamBounds(out offset, out length);
        }

        /// <summary>
        /// Consumes the terminating character for the current control sequence.
        /// </summary>
        public bool EndSequence()
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
