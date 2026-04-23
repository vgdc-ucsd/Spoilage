using UnityEngine;

namespace TextboxControl
{
    /// <summary>
    /// Baseline text styling representation managed by reducer methods.
    /// </summary>
    public struct CharBaseState
    {
        public bool Bold;
        public bool Italic;
        public bool Underline;
        public bool Strikethrough;
        public Color Color;
        public bool HasColor;
        public string FontName;
        public float SizeOverride;
        public float CharSpacing;
        public float BaselineOffset;

        public static CharBaseState Default => new CharBaseState { SizeOverride = float.NaN };

        public bool IsDefault =>
            !Bold && !Italic && !Underline && !Strikethrough &&
            !HasColor && FontName == null &&
            float.IsNaN(SizeOverride) &&
            CharSpacing == 0 && BaselineOffset == 0;
    }

    /// <summary>
    /// Consecutive character run sharing the same <see cref="CharBaseState"/>.
    /// </summary>
    public struct StyleRun
    {
        public int Count;
        public CharBaseState Style;
    }
}
