using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TextboxControl
{
    public static class TMPFormatter
    {
        [System.ThreadStatic] static StringBuilder _sb;

        public static string Build(IReadOnlyList<char> buffer, IReadOnlyList<StyleRun> runs)
        {
            StringBuilder sb = _sb ?? (_sb = new StringBuilder(256));
            sb.Clear();

            int charIndex = 0;
            bool anyOpen = false;
            CharBaseState previous = CharBaseState.Default;

            for (int r = 0; r < runs.Count; r++)
            {
                StyleRun run = runs[r];

                if (anyOpen)
                    CloseAll(sb, previous);

                OpenFor(sb, run.Style);
                anyOpen = !run.Style.IsDefault;
                previous = run.Style;

                int end = charIndex + run.Count;
                for (; charIndex < end; charIndex++)
                {
                    AppendEscaped(sb, buffer[charIndex]);
                }
            }

            if (anyOpen)
            {
                CloseAll(sb, previous);
            }

            return sb.ToString();
        }

        static void OpenFor(StringBuilder sb, CharBaseState state)
        {
            if (state.HasColor)
            {
                sb.Append("<color=#").Append(ColorToHex(state.Color)).Append('>');
            }
            if (!float.IsNaN(state.SizeOverride))
            {
                sb.Append("<size=").Append(state.SizeOverride.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append('>');
            }
            if (state.CharSpacing != 0)
            {
                sb.Append("<cspace=").Append(state.CharSpacing.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append("em>");
            }
            if (state.BaselineOffset != 0)
            {
                sb.Append("<voffset=").Append(state.BaselineOffset.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append("em>");
            }
            if (state.FontName != null)
            {
                sb.Append("<font=\"").Append(state.FontName).Append("\">");
            }
            if (state.Bold)
            {
                sb.Append("<b>");
            }
            if (state.Italic)
            {
                sb.Append("<i>");
            }
            if (state.Underline)
            {
                sb.Append("<u>");
            }
            if (state.Strikethrough)
            {
                sb.Append("<s>");
            }
        }

        static void CloseAll(StringBuilder sb, CharBaseState state)
        {
            if (state.Strikethrough)
            {
                sb.Append("</s>");
            }
            if (state.Underline)
            {
                sb.Append("</u>");
            }
            if (state.Italic)
            {
                sb.Append("</i>");
            }
            if (state.Bold)
            {
                sb.Append("</b>");
            }
            if (state.FontName != null)
            {
                sb.Append("</font>");
            }
            if (state.BaselineOffset != 0)
            {
                sb.Append("</voffset>");
            }
            if (state.CharSpacing != 0)
            {
                sb.Append("</cspace>");
            }
            if (!float.IsNaN(state.SizeOverride))
            {
                sb.Append("</size>");
            }
            if (state.HasColor)
            {
                sb.Append("</color>");
            }
        }

        static void AppendEscaped(StringBuilder sb, char c)
        {
            if (c == '<')
            {
                sb.Append("<noparse><</noparse>");
            }
            else
            {
                sb.Append(c);
            }
        }

        static string ColorToHex(Color c)
        {
            byte r = (byte)Mathf.Clamp(Mathf.RoundToInt(c.r * 255), 0, 255);
            byte g = (byte)Mathf.Clamp(Mathf.RoundToInt(c.g * 255), 0, 255);
            byte b = (byte)Mathf.Clamp(Mathf.RoundToInt(c.b * 255), 0, 255);
            byte a = (byte)Mathf.Clamp(Mathf.RoundToInt(c.a * 255), 0, 255);

            return a == 255
                ? $"{r:X2}{g:X2}{b:X2}"
                : $"{r:X2}{g:X2}{b:X2}{a:X2}";
        }
    }
}
