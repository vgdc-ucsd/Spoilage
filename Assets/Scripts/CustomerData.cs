using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "ScriptableObjects/CustomerData", order = 1)]
public class CustomerData : ScriptableObject
{
    public const int NUM_SPRITES = 16;
    public enum Spoilage
    {
        NOT,
        SLIGHTLY,
        VERY
    }
    public enum Indexes
    {
        BODY,
        CLOTHES,
        MOUTH_OPEN,
        MOUTH_CLOSED,
        MOUTH_ANGER,
        MOUTH_DISGUST,
        EYES_OPEN,
        EYES_CLOSED,
        EYES_ANGER,
        EYES_DISGUST,
        EYES_WIDENING,
        HAIR_FRONT,
        HAIR_BACK,
        HAIR_SHADOW,
        SPOILAGE_FRONT,
        SPOILAGE_BACK,
    }

    public Sprite[] sprites;
    public Vector3 faceOffset;
    //public Vector3[] spriteOffsets;
    public string CustomerName;
    public Spoilage spoilage;
    public float patience;

}
