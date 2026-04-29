using UnityEngine;

public class CustomerData : ScriptableObject
{
    public const int NUM_SPRITES = 8;
    public enum Indexes
    {
        BODY,
        HAIR_FRONT,
        HAIR_BACK,
        // etc.
    }
    public Sprite[] sprites;
    public Vector3[] spriteOffsets;
    public float spoilage; // What range of values do we want for this?
}