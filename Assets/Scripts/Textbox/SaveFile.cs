using System;
using System.Collections.Generic;

namespace TextboxControl
{
    /// <summary>
    /// In-memory view over a parsed dialogue save text file.
    /// </summary>
    /// <remarks>
    /// Format:
    /// non-indented lines ending with ':' declare sequence names; following tab-indented lines are boxes.
    /// </remarks>
    public class DialogueSaveFile
    {
        private readonly string _text;
        private readonly Entry[] _entries;

        private struct BoxSpan
        {
            public int Start;
            public int Length;
        }

        private struct Entry
        {
            public int NameStart;
            public int NameLength;
            public BoxSpan[] Boxes;
        }

        private DialogueSaveFile(string text, Entry[] entries)
        {
            _text = text;
            _entries = entries;
        }

        /// <summary>
        /// Returns the number of boxes for a sequence, or -1 when the sequence is missing.
        /// </summary>
        public int CountBoxes(string name)
        {
            int index = FindEntryIndex(name);
            return index >= 0 ? _entries[index].Boxes.Length : -1;
        }

        /// <summary>
        /// Returns one dialogue box by sequence name and index, or null when missing/out of range.
        /// </summary>
        public string GetBox(string name, int boxIndex)
        {
            int entryIndex = FindEntryIndex(name);
            if (entryIndex < 0)
            {
                return null;
            }

            ref Entry entry = ref _entries[entryIndex];
            if ((uint)boxIndex >= (uint)entry.Boxes.Length)
            {
                return null;
            }

            ref BoxSpan box = ref entry.Boxes[boxIndex];
            return _text.Substring(box.Start, box.Length);
        }

        /// <summary>
        /// Parses raw save text into a lookup-friendly representation.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
        /// <exception cref="FormatException">Thrown when the file structure is invalid.</exception>
        public static DialogueSaveFile Parse(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            List<Entry> entries = new List<Entry>();
            List<BoxSpan> currentBoxes = new List<BoxSpan>();

            int nameStart = -1;
            int nameLen = 0;

            int textLength = text.Length;
            int lineStart = 0;

            for (int i = 0; i <= textLength; i++)
            {
                bool atEnd = i == textLength;
                bool isLineBreak = !atEnd && (text[i] == '\n' || text[i] == '\r');
                if (!atEnd && !isLineBreak)
                {
                    continue;
                }

                int lineOff = lineStart;
                int lineLen = i - lineOff;

                if (!atEnd && text[i] == '\r' && i + 1 < textLength && text[i + 1] == '\n')
                {
                    i++;
                }

                lineStart = i + 1;

                if (lineLen == 0 || text[lineOff] == '#')
                {
                    continue;
                }

                if (text[lineOff] == '\t')
                {
                    if (nameStart < 0)
                    {
                        throw new FormatException($"Indented line at position {lineOff} has no preceding sequence name.");
                    }

                    currentBoxes.Add(new BoxSpan
                    {
                        Start = lineOff + 1,
                        Length = lineLen - 1,
                    });
                    continue;
                }

                if (text[lineOff + lineLen - 1] != ':')
                {
                    throw new FormatException($"Expected sequence header ending in ':' at position {lineOff}.");
                }

                int curNameLen = lineLen - 1;
                if (!IsValidName(text, lineOff, curNameLen))
                {
                    throw new FormatException($"Invalid sequence name at position {lineOff}.");
                }

                if (nameStart >= 0)
                {
                    entries.Add(CreateEntry(nameStart, nameLen, currentBoxes));
                    currentBoxes.Clear();
                }

                nameStart = lineOff;
                nameLen = curNameLen;
            }

            if (nameStart >= 0)
            {
                entries.Add(CreateEntry(nameStart, nameLen, currentBoxes));
            }

            return new DialogueSaveFile(text, entries.ToArray());
        }

        private int FindEntryIndex(string name)
        {
            if (name == null)
            {
                return -1;
            }

            for (int i = 0; i < _entries.Length; i++)
            {
                ref Entry entry = ref _entries[i];
                if (SpanEquals(name, entry.NameStart, entry.NameLength))
                {
                    return i;
                }
            }

            return -1;
        }

        private static Entry CreateEntry(int nameStart, int nameLength, List<BoxSpan> boxes)
        {
            return new Entry
            {
                NameStart = nameStart,
                NameLength = nameLength,
                Boxes = boxes.ToArray(),
            };
        }

        private bool SpanEquals(string name, int start, int length)
        {
            return name.Length == length &&
                   string.CompareOrdinal(name, 0, _text, start, length) == 0;
        }

        private static bool IsValidName(string s, int start, int length)
        {
            if (length == 0)
            {
                return false;
            }

            char first = s[start];
            if (!(char.IsLetter(first) || first == '_'))
            {
                return false;
            }

            int end = start + length;
            for (int i = start + 1; i < end; i++)
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
