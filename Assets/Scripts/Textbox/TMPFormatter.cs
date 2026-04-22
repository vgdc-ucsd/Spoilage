using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace TextboxControl
{
    public static class TMPFormatter
    {
        [System.ThreadStatic]
        private static StringBuilder _builder;

        public static string Build(IReadOnlyList<char> buffer, IReadOnlyList<StyleRun> runs)
        {
            if (buffer == null || buffer.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder builder = GetBuilder();
            int charIndex = 0;
            bool hasOpenTags = false;
            CharBaseState previousStyle = CharBaseState.Default;

            for (int runIndex = 0; runIndex < runs.Count; runIndex++)
            {
                StyleRun run = runs[runIndex];

                if (hasOpenTags)
                {
                    CloseAll(builder, previousStyle);
                }

                OpenFor(builder, run.Style);
                hasOpenTags = !run.Style.IsDefault;
                previousStyle = run.Style;

                int runEnd = charIndex + run.Count;
                while (charIndex < runEnd)
                {
                    AppendEscaped(builder, buffer[charIndex]);
                    charIndex++;
                }
            }

            if (hasOpenTags)
            {
                CloseAll(builder, previousStyle);
            }

            return builder.ToString();
        }

        private static StringBuilder GetBuilder()
        {
            StringBuilder builder = _builder;
            if (builder == null)
            {
                builder = new StringBuilder(256);
                _builder = builder;
            }

            builder.Clear();
            return builder;
        }

        private static void OpenFor(StringBuilder builder, CharBaseState state)
        {
            if (state.HasColor)
            {
                builder.Append("<color=#").Append(ColorToHex(state.Color)).Append('>');
            }

            if (!float.IsNaN(state.SizeOverride))
            {
                builder.Append("<size=")
                    .Append(state.SizeOverride.ToString(CultureInfo.InvariantCulture))
                    .Append('>');
            }

            if (state.CharSpacing != 0f)
            {
                builder.Append("<cspace=")
                    .Append(state.CharSpacing.ToString(CultureInfo.InvariantCulture))
                    .Append("em>");
            }

            if (state.BaselineOffset != 0f)
            {
                builder.Append("<voffset=")
                    .Append(state.BaselineOffset.ToString(CultureInfo.InvariantCulture))
                    .Append("em>");
            }

            if (state.FontName != null)
            {
                builder.Append("<font=\"").Append(state.FontName).Append("\">");
            }

            if (state.Bold)
            {
                builder.Append("<b>");
            }

            if (state.Italic)
            {
                builder.Append("<i>");
            }

            if (state.Underline)
            {
                builder.Append("<u>");
            }

            if (state.Strikethrough)
            {
                builder.Append("<s>");
            }
        }

        private static void CloseAll(StringBuilder builder, CharBaseState state)
        {
            if (state.Strikethrough)
            {
                builder.Append("</s>");
            }

            if (state.Underline)
            {
                builder.Append("</u>");
            }

            if (state.Italic)
            {
                builder.Append("</i>");
            }

            if (state.Bold)
            {
                builder.Append("</b>");
            }

            if (state.FontName != null)
            {
                builder.Append("</font>");
            }

            if (state.BaselineOffset != 0f)
            {
                builder.Append("</voffset>");
            }

            if (state.CharSpacing != 0f)
            {
                builder.Append("</cspace>");
            }

            if (!float.IsNaN(state.SizeOverride))
            {
                builder.Append("</size>");
            }

            if (state.HasColor)
            {
                builder.Append("</color>");
            }
        }

        private static void AppendEscaped(StringBuilder builder, char c)
        {
            if (c == '<')
            {
                builder.Append("<noparse><</noparse>");
                return;
            }

            builder.Append(c);
        }

        private static string ColorToHex(Color color)
        {
            byte r = (byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255);
            byte g = (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255);
            byte b = (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255);
            byte a = (byte)Mathf.Clamp(Mathf.RoundToInt(color.a * 255f), 0, 255);

            return a == 255
                ? $"{r:X2}{g:X2}{b:X2}"
                : $"{r:X2}{g:X2}{b:X2}{a:X2}";
        }
    }
}
