using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

namespace TextboxControl.Animation
{
    /// <summary>
    /// Creates animation instances and binds optional inline parameters.
    /// </summary>
    public static class AnimationRegistry
    {
        private static readonly Dictionary<string, Func<IAnimation>> Factories =
            new Dictionary<string, Func<IAnimation>>(StringComparer.Ordinal)
            {
                { "wavy", () => new WavyAnim() },
                { "jitter", () => new JitterAnim() },
                { "rainbow", () => new RainbowAnim() },
                { "shake", () => new ShakeAnim() },
                { "pulse", () => new PulseAnim() },
                { "spin", () => new SpinAnim() },
                { "arc", () => new ArcAnim() },
                { "fade", () => new FadeAnim() },
            };

        private static readonly Dictionary<Type, FieldInfo[]> ParamFieldCache =
            new Dictionary<Type, FieldInfo[]>();

        /// <summary>
        /// True when an animation factory exists for <paramref name="name"/>.
        /// </summary>
        public static bool IsRegistered(string name)
        {
            return name != null && Factories.ContainsKey(name);
        }

        public static IEnumerable<string> RegisteredNames => Factories.Keys;

        /// <summary>
    /// Creates and configures an animation instance from parsed control parameters.
        /// </summary>
        public static IAnimation Create(string animName, string source, List<(int offset, int length)> paramSpans)
        {
            if (animName == null || !Factories.TryGetValue(animName, out Func<IAnimation> factory))
            {
                return null;
            }

            IAnimation instance;
            try
            {
                instance = factory();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[TextboxControl] Failed to construct \"{animName}\": {e.Message}");
                return null;
            }

            if (paramSpans != null && paramSpans.Count > 0)
            {
                BindParams(instance, source, paramSpans);
            }

            return instance;
        }

        private static FieldInfo[] GetParamFields(Type type)
        {
            if (ParamFieldCache.TryGetValue(type, out FieldInfo[] cached))
            {
                return cached;
            }

            List<FieldInfo> fields = new List<FieldInfo>();
            for (Type current = type; current != null && current != typeof(object); current = current.BaseType)
            {
                FieldInfo[] declared = current.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                for (int i = 0; i < declared.Length; i++)
                {
                    FieldInfo field = declared[i];
                    if (Attribute.IsDefined(field, typeof(ParamAttribute)))
                    {
                        fields.Add(field);
                    }
                }
            }

            FieldInfo[] result = fields.ToArray();
            ParamFieldCache[type] = result;
            return result;
        }

        private static void BindParams(IAnimation instance, string source, List<(int offset, int length)> spans)
        {
            FieldInfo[] fields = GetParamFields(instance.GetType());
            if (fields.Length == 0)
            {
                return;
            }

            for (int si = 0; si < spans.Count; si++)
            {
                (int spanOffset, int spanLength) = spans[si];
                int spanEnd = spanOffset + spanLength;
                int pieceOff = spanOffset;

                while (pieceOff < spanEnd)
                {
                    int pieceEnd = pieceOff;
                    while (pieceEnd < spanEnd && source[pieceEnd] != ',')
                    {
                        pieceEnd++;
                    }

                    TryBindParamPiece(instance, fields, source.AsSpan(pieceOff, pieceEnd - pieceOff));
                    pieceOff = pieceEnd + 1;
                }
            }
        }

        private static void TryBindParamPiece(IAnimation instance, FieldInfo[] fields, ReadOnlySpan<char> piece)
        {
            int eq = piece.IndexOf('=');
            if (eq <= 0 || eq >= piece.Length - 1)
            {
                return;
            }

            ReadOnlySpan<char> key = piece.Slice(0, eq);
            ReadOnlySpan<char> value = piece.Slice(eq + 1);

            for (int fi = 0; fi < fields.Length; fi++)
            {
                FieldInfo field = fields[fi];
                if (!key.Equals(field.Name.AsSpan(), StringComparison.Ordinal))
                {
                    continue;
                }

                if (TryConvertSpan(value, field.FieldType, out object converted))
                {
                    field.SetValue(instance, converted);
                }
                else
                {
                    Debug.LogWarning($"[TextboxControl] Could not convert \"{value.ToString()}\" to {field.FieldType.Name} for param '{field.Name}'.");
                }

                return;
            }
        }

        private static bool TryConvertSpan(ReadOnlySpan<char> raw, Type target, out object result)
        {
            result = null;

            if (target == typeof(float))
            {
                if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                {
                    result = value;
                    return true;
                }

                return false;
            }

            if (target == typeof(int))
            {
                if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                {
                    result = value;
                    return true;
                }

                return false;
            }

            if (target == typeof(bool))
            {
                if (raw.Equals("1".AsSpan(), StringComparison.Ordinal) ||
                    raw.Equals("true".AsSpan(), StringComparison.OrdinalIgnoreCase) ||
                    raw.Equals("on".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    return true;
                }

                if (raw.Equals("0".AsSpan(), StringComparison.Ordinal) ||
                    raw.Equals("false".AsSpan(), StringComparison.OrdinalIgnoreCase) ||
                    raw.Equals("off".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    result = false;
                    return true;
                }

                return false;
            }

            if (target == typeof(string))
            {
                result = raw.ToString();
                return true;
            }

            if (target.IsEnum)
            {
                try
                {
                    result = Enum.Parse(target, raw.ToString(), ignoreCase: true);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
    }
}
