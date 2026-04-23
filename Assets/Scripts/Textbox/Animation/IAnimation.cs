using UnityEngine;

namespace TextboxControl.Animation
{
    /// <summary>
    /// Stateless per-character animation operator.
    /// </summary>
    public interface IAnimation
    {
        void Apply(ref CharAnimState state, AnimationContext ctx);
    }

    /// <summary>
    /// Optional extension point for animations that need direct quad vertex access.
    /// </summary>
    public interface IVertexAnimation : IAnimation
    {
        void ApplyVertices(Vector3[] quadVerts, Color32[] quadColors, AnimationContext ctx);
    }

    /// <summary>
    /// Marks animation fields that can be bound from inline `key=value` control params.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = false)]
    public sealed class ParamAttribute : System.Attribute { }

    /// <summary>
    /// Runtime metadata provided to animation callbacks.
    /// </summary>
    public struct AnimationContext
    {
        public int CharIndexInBuffer;
        public int CharIndexInRegion;
        public int RegionLength;
        public float TimeSinceRegionStart;
    }

    /// <summary>
    /// Transform/tint delta accumulated from one or more animations for a single character.
    /// </summary>
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
