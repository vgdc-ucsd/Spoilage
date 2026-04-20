using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

namespace TextboxControl.Animation
{
    public static class AnimationRegistry
    {
        static readonly Dictionary<string, Func<IAnimation>> _factories =
            new Dictionary<string, Func<IAnimation>>(StringComparer.Ordinal)
        {
            { "wavy",    () => new WavyAnim()    },
            { "jitter",  () => new JitterAnim()  },
            { "rainbow", () => new RainbowAnim() },
            { "shake",   () => new ShakeAnim()   },
            { "pulse",   () => new PulseAnim()   },
            { "spin",    () => new SpinAnim()    },
            { "arc",     () => new ArcAnim()     },
            { "fade",    () => new FadeAnim()    },
        };

        static readonly Dictionary<Type, FieldInfo[]> _fieldCache =
            new Dictionary<Type, FieldInfo[]>();

        public static bool IsRegistered(string name)
            => name != null && _factories.ContainsKey(name);

        public static IEnumerable<string> RegisteredNames => _factories.Keys;

        public static IAnimation Create(string animName,
                                        string source,
                                        List<(int offset, int length)> paramSpans)
        {
            if (animName == null || !_factories.TryGetValue(animName, out Func<IAnimation> factory))
            {
                return null;
            }

            IAnimation inst;
            try { inst = factory(); }
            catch (Exception e)
            {
                Debug.LogWarning($"[TextboxControl] Failed to construct \"{animName}\": {e.Message}");
                return null;
            }

            if (paramSpans != null && paramSpans.Count > 0)
            {
                BindParams(inst, source, paramSpans);
            }

            return inst;
        }

        static FieldInfo[] GetParamFields(Type t)
        {
            if (_fieldCache.TryGetValue(t, out FieldInfo[] cached))
            {
                return cached;
            }

            List<FieldInfo> list = new List<FieldInfo>();
            for (Type cur = t; cur != null && cur != typeof(object); cur = cur.BaseType)
            {
                foreach (FieldInfo f in cur.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (Attribute.IsDefined(f, typeof(ParamAttribute)))
                    {
                        list.Add(f);
                    }
                }
            }
            FieldInfo[] arr = list.ToArray();
            _fieldCache[t] = arr;
            return arr;
        }

        static void BindParams(IAnimation inst, string src, List<(int offset, int length)> spans)
        {
            FieldInfo[] fields = GetParamFields(inst.GetType());
            if (fields.Length == 0)
            {
                return;
            }

            for (int si = 0; si < spans.Count; si++)
            {
                (int spanOff, int spanLen) = spans[si];
                int spanEnd = spanOff + spanLen;
                int cursor = spanOff;

                while (cursor < spanEnd)
                {
                    int pieceEnd = cursor;
                    while (pieceEnd < spanEnd && src[pieceEnd] != ',')
                    {
                        pieceEnd++;
                    }

                    int eq = cursor;
                    while (eq < pieceEnd && src[eq] != '=')
                    {
                        eq++;
                    }

                    if (eq > cursor && eq < pieceEnd - 1)
                    {
                        ReadOnlySpan<char> key = src.AsSpan(cursor, eq - cursor);
                        ReadOnlySpan<char> val = src.AsSpan(eq + 1, pieceEnd - eq - 1);

                        for (int fi = 0; fi < fields.Length; fi++)
                        {
                            FieldInfo f = fields[fi];
                            if (!key.Equals(f.Name.AsSpan(), StringComparison.Ordinal))
                            {
                                continue;
                            }
                            if (TryConvertSpan(val, f.FieldType, out object converted))
                            {
                                f.SetValue(inst, converted);
                            }
                            else
                            {
                                Debug.LogWarning($"[TextboxControl] Could not convert \"{val.ToString()}\" to {f.FieldType.Name} for param '{f.Name}'.");
                            }
                            break;
                        }
                    }

                    cursor = pieceEnd + 1;
                }
            }
        }

        static bool TryConvertSpan(ReadOnlySpan<char> raw, Type target, out object result)
        {
            result = null;
            if (target == typeof(float))
            {
                if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float v))
                {
                    result = v;
                    return true;
                }
                return false;
            }
            if (target == typeof(int))
            {
                if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v))
                {
                    result = v;
                    return true;
                }
                return false;
            }
            if (target == typeof(bool))
            {
                if (raw.Equals("1".AsSpan(), StringComparison.Ordinal) ||
                    raw.Equals("true".AsSpan(), StringComparison.Ordinal) ||
                    raw.Equals("on".AsSpan(), StringComparison.Ordinal))
                {
                    result = true;
                    return true;
                }
                if (raw.Equals("0".AsSpan(), StringComparison.Ordinal) ||
                    raw.Equals("false".AsSpan(), StringComparison.Ordinal) ||
                    raw.Equals("off".AsSpan(), StringComparison.Ordinal))
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
