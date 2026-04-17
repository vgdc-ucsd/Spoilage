using UnityEngine;

public class CustomerData : ScriptableObject
{
    public const int NUM_SPRITES = 11;
    public enum Spoilage
    {
        NOT,
        SLIGHTLY,
        VERY
    }
    public enum Indexes
    {
        BODY,
        HAIR_FRONT,
        HAIR_BACK,
        HAIR_SHADOW,
        CLOTHES,
        NOSE_MOUTH_OPEN,
        NOSE_MOUTH_CLOSED,
        EYES_OPEN,
        EYES_CLOSED,
        SPOILAGE_FRONT,
        SPOILAGE_BACK
        // etc.
    }
    
    public Sprite[] sprites;
    //public Vector3[] spriteOffsets;
    public Spoilage spoilage; 
    public float patience;


}
