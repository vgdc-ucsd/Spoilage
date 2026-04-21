using System.Collections.Generic;

namespace TextboxControl
{
    public class DialogueSaveFile
    {
        private readonly string _text;
        private readonly Entry[] _entries;

        struct BoxSpan
        {
            public int Start;
            public int Len;
        }

        struct Entry
        {
            public int NameStart;
            public int NameLen;
            public BoxSpan[] Boxes;
        }

        DialogueSaveFile(string text, Entry[] entries)
        {
            _text = text;
            _entries = entries;
        }

        public int CountBoxes(string name)
        {
            for (int i = 0; i < _entries.Length; i++)
            {
                ref Entry e = ref _entries[i];
                if (SpanEquals(name, e.NameStart, e.NameLen))
                {
                    return e.Boxes.Length;
                }
            }
            return -1;
        }

        public string GetBox(string name, int boxIndex)
        {
            for (int i = 0; i < _entries.Length; i++)
            {
                ref Entry e = ref _entries[i];
                if (!SpanEquals(name, e.NameStart, e.NameLen))
                {
                    continue;
                }
                if ((uint)boxIndex >= (uint)e.Boxes.Length)
                {
                    return null;
                }
                ref BoxSpan b = ref e.Boxes[boxIndex];
                return _text.Substring(b.Start, b.Len);
            }
            return null;
        }

        private bool SpanEquals(string name, int start, int len)
        {
            return name.Length == len && string.CompareOrdinal(name, 0, _text, start, len) == 0;
        }

        public static DialogueSaveFile Parse(string text)
        {
            List<Entry> entries = new List<Entry>();
            List<BoxSpan> currentBoxes = new List<BoxSpan>();
            int currentNameStart = -1;
            int currentNameLen = 0;
            int len = text.Length;
            int lineStart = 0;

            for (int i = 0; i <= len; i++)
            {
                bool atEnd = i == len;
                bool isNewline = !atEnd && (text[i] == '\n' || text[i] == '\r');
                if (!atEnd && !isNewline)
                {
                    continue;
                }

                int thisLineStart = lineStart;
                int lineLen = i - thisLineStart;
                if (!atEnd && text[i] == '\r' && i + 1 < len && text[i + 1] == '\n')
                {
                    i++;
                }
                lineStart = i + 1;

                if (lineLen == 0 || text[thisLineStart] == '#')
                {
                    continue;
                }

                if (text[thisLineStart] == '\t')
                {
                    if (currentNameStart < 0)
                    {
                        throw new System.FormatException($"Indented line at position {thisLineStart} has no preceding sequence name.");
                    }
                    currentBoxes.Add(new BoxSpan { Start = thisLineStart + 1, Len = lineLen - 1 });
                    continue;
                }

                if (text[thisLineStart + lineLen - 1] != ':')
                {
                    throw new System.FormatException($"Expected sequence header ending in ':' at position {thisLineStart}.");
                }

                int nameLen = lineLen - 1;
                if (!IsValidName(text, thisLineStart, nameLen))
                {
                    throw new System.FormatException($"Invalid sequence name at position {thisLineStart}.");
                }

                if (currentNameStart >= 0)
                {
                    entries.Add(new Entry
                    {
                        NameStart = currentNameStart,
                        NameLen = currentNameLen,
                        Boxes = currentBoxes.ToArray(),
                    });
                }

                currentNameStart = thisLineStart;
                currentNameLen = nameLen;
                currentBoxes.Clear();
            }

            if (currentNameStart >= 0)
            {
                entries.Add(new Entry
                {
                    NameStart = currentNameStart,
                    NameLen = currentNameLen,
                    Boxes = currentBoxes.ToArray(),
                });
            }

            return new DialogueSaveFile(text, entries.ToArray());
        }

        private static bool IsValidName(string s, int start, int len)
        {
            if (len == 0)
            {
                return false;
            }

            char c0 = s[start];
            if (!(char.IsLetter(c0) || c0 == '_'))
            {
                return false;
            }

            for (int i = start + 1; i < start + len; i++)
            {
                char c = s[i];
                if (!(char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.'))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
