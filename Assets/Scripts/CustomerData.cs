using UnityEngine;

public class CustomerData : ScriptableObject
{
    public const int NUM_SPRITES = 9;

    /* Enum is actually kind of hard to work with since it can't be implicitly cast to int to actually use as indexes
    public const int BODY_IDX = 0;
    public const int HAIR_FRONT_IDX = 1;
    public const int HAIR_BACK_IDX = 2;
    public const int HAIR_SHADOW_IDX = 3;
    public const int CLOTHES_IDX = 4;
    public const int NOSE_MOUTH_IDX = 5;
    public const int EYES_IDX = 6;
    public const int SPOILAGE_FRONT_IDX = 7;
    public const int SPOILAGE_BACK_IDX = 8;
    */
    public enum Indexes
    {
        BODY = 0,
        HAIR_FRONT,
        HAIR_BACK,
        HAIR_SHADOW,
        CLOTHES,
        NOSE_MOUTH,
        EYES,
        SPOILAGE_FRONT,
        SPOILAGE_BACK
        // etc.
    }
    
    public Sprite[] sprites;
    public Vector3[] spriteOffsets;
    public int spoilage; 
}
