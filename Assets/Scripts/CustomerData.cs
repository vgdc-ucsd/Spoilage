using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "ScriptableObjects/CustomerData", order = 1)]
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
        SPOILAGE_BACK,
        HAIR_BACK,
        BODY,
        NOSE_MOUTH_OPEN,
        NOSE_MOUTH_CLOSED,
        EYES_OPEN,
        EYES_CLOSED,
        CLOTHES,
        HAIR_SHADOW,
        HAIR_FRONT,
        SPOILAGE_FRONT
    }

    public Sprite[] sprites;
    //public Vector3[] spriteOffsets;
    public string CustomerName;
    public Spoilage spoilage;
    public float patience;


}
