namespace TextboxControl
{
    /// <summary>
    /// A character range in the revealed buffer, possibly with animation payload.
    /// </summary>
    public class Region
    {
        public int Id;
        public string Name;
        public int Start;
        public int Length;
        public double StartTime;

        public Animation.IAnimation Animation;

        public int End => Start + Length;

        public bool Contains(int bufferIndex) => bufferIndex >= Start && bufferIndex < Start + Length;
    }
}