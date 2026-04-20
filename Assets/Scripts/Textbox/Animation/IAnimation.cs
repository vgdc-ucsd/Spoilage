using UnityEngine;

namespace TextboxControl.Animation
{
    public interface IAnimation
    {
        void Apply(ref CharAnimState state, AnimationContext ctx);
    }

    public interface IVertexAnimation : IAnimation
    {
        void ApplyVertices(Vector3[] quadVerts, Color32[] quadColors, AnimationContext ctx);
    }

    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false)]
    public sealed class ParamAttribute : System.Attribute { }

    public struct AnimationContext
    {
        public int CharIndexInBuffer;
        public int CharIndexInRegion;
        public int RegionLength;
        public float TimeSinceRegionStart;
    }

    public struct CharAnimState
    {
        public Vector2 PositionOffset;
        public float Rotation;
        public Vector2 Scale;
        public Color ColorTint;

        public static CharAnimState Identity => new CharAnimState
        {
            PositionOffset = Vector2.zero,
            Rotation = 0f,
            Scale = Vector2.one,
            ColorTint = Color.white,
        };
    }
}
