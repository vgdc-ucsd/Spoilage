using UnityEngine;

namespace TextboxControl.Animation
{
    public class WavyAnim : IAnimation
    {
        [Param] public float amp = 2f;
        [Param] public float freq = 2f;
        [Param] public float phasePerChar = 0.5f;

        public void Apply(ref CharAnimState s, AnimationContext ctx)
        {
            float t = ctx.TimeSinceRegionStart * freq * 2f * Mathf.PI
                    + ctx.CharIndexInRegion * phasePerChar;
            s.PositionOffset.y += amp * Mathf.Sin(t);
        }
    }

    public class JitterAnim : IAnimation
    {
        [Param] public float amp = 1f;
        [Param] public float hz = 15f;

        public void Apply(ref CharAnimState s, AnimationContext ctx)
        {
            int cycle = (int)(ctx.TimeSinceRegionStart * hz);
            int sx = Hash(cycle * 131 + ctx.CharIndexInRegion * 17);
            int sy = Hash(cycle * 131 + ctx.CharIndexInRegion * 17 + 91);
            s.PositionOffset.x += ((sx & 0xFFFF) / 32767.5f - 1f) * amp / 4;
            s.PositionOffset.y += ((sy & 0xFFFF) / 32767.5f - 1f) * amp / 4;
        }

        static int Hash(int x)
        {
            x = ((x >> 16) ^ x) * 0x45d9f3b;
            x = ((x >> 16) ^ x) * 0x45d9f3b;
            return (x >> 16) ^ x;
        }
    }

    public class RainbowAnim : IAnimation
    {
        [Param] public float speed = 0.5f;
        [Param] public float phasePerChar = 0.1f;
        [Param] public float saturation = 1f;
        [Param] public float value = 1f;

        public void Apply(ref CharAnimState s, AnimationContext ctx)
        {
            float hue = (ctx.TimeSinceRegionStart * speed + ctx.CharIndexInRegion * phasePerChar) % 1f;
            if (hue < 0)
            {
                hue += 1f;
            }

            Color c = Color.HSVToRGB(hue, saturation, value);
            s.ColorTint.r *= c.r;
            s.ColorTint.g *= c.g;
            s.ColorTint.b *= c.b;
        }
    }

    public class ShakeAnim : IAnimation
    {
        [Param] public float amp = 1f;
        [Param] public float hz = 20f;

        public void Apply(ref CharAnimState s, AnimationContext ctx)
        {
            int cycle = (int)(ctx.TimeSinceRegionStart * hz);
            int sx = Hash(cycle * 311 + 7);
            int sy = Hash(cycle * 311 + 251);
            s.PositionOffset.x += ((sx & 0xFFFF) / 32767.5f - 1f) * amp;
            s.PositionOffset.y += ((sy & 0xFFFF) / 32767.5f - 1f) * amp;
        }
        static int Hash(int x)
        {
            x = ((x >> 16) ^ x) * 0x45d9f3b;
            x = ((x >> 16) ^ x) * 0x45d9f3b;
            return (x >> 16) ^ x;
        }
    }

    public class PulseAnim : IAnimation
    {
        [Param] public float amp = 0.2f;
        [Param] public float hz = 2f;

        public void Apply(ref CharAnimState s, AnimationContext ctx)
        {
            float f = 1f + amp * Mathf.Sin(ctx.TimeSinceRegionStart * hz * 2f * Mathf.PI);
            s.Scale.x *= f;
            s.Scale.y *= f;
        }
    }

    public class SpinAnim : IAnimation
    {
        [Param] public float degPerSec = 90f;
        [Param] public float phasePerChar = 30f;

        public void Apply(ref CharAnimState s, AnimationContext ctx)
        {
            s.Rotation -= ctx.TimeSinceRegionStart * degPerSec
                        + ctx.CharIndexInRegion * phasePerChar;
        }
    }

    public class ArcAnim : IAnimation
    {
        [Param] public float amp = 3f;
        [Param] public float period = 0.6f;
        [Param] public float staggerPerChar = 0.08f;
        [Param] public bool loop = true;

        public void Apply(ref CharAnimState s, AnimationContext ctx)
        {
            float localT = (ctx.TimeSinceRegionStart - ctx.CharIndexInRegion * staggerPerChar) / period;
            if (loop)
            {
                localT -= Mathf.Floor(localT);
            }
            else if (localT < 0f || localT > 1f)
            {
                return;
            }
            float y = localT * (1f - localT);
            s.PositionOffset.y += y * amp;
        }
    }

    public class FadeAnim : IAnimation
    {
        [Param] public float duration = 0.5f;
        [Param] public string direction = "in";

        public void Apply(ref CharAnimState s, AnimationContext ctx)
        {
            float a = duration <= 0f ? 1f : Mathf.Clamp01(ctx.TimeSinceRegionStart / duration);
            if (direction == "out")
            {
                a = 1f - a;
            }
            s.ColorTint.a *= a;
        }
    }
}
