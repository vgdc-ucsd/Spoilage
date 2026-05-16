using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "ScriptableObjects/CustomerData", order = 1)]
public class CustomerData : ScriptableObject
{
    public const int NUM_SPRITES = 16;
    public enum Spoilage
    {
        UNSPOILED,
        STAGE_I,
        STAGE_II
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

    /// <summary>
    /// Story importance class. Key characters appear in
    /// <see cref="DayEntry.beginInteraction"/> or <see cref="DayEntry.endInteraction"/>
    /// slots. SemiKey characters belong to a <see cref="SemiKeySet"/> (or the
    /// additional pool) and appear during cooking alongside random customers,
    /// and may also occupy interaction slots.
    /// </summary>
    public enum Tier
    {
        None,
        Key,
        SemiKey
    }

    public Sprite[] sprites;
    public Vector3 faceOffset;
    public Vector3 eyeOffset;
    //public Vector3[] spriteOffsets;

    public Spoilage spoilage;
    public AbstractSpoilageSymptom spoilageSymtomp;
    public float patience;

    public List<CustomerOrder> orders = new List<CustomerOrder>();

    /// <summary>
    /// Identifier for story-relevant characters, empty for randomly generated
    /// customers. Used as the dialogue-key prefix. Multiple <see cref="CustomerData"/>
    /// assets sharing one id are different appearances of the same character.
    /// </summary>
    public string id;

    /// <summary>
    /// Story importance class. <see cref="Tier.Key"/> and
    /// <see cref="Tier.SemiKey"/> entries must also have a non-empty
    /// <see cref="id"/>.
    /// </summary>
    public Tier tier = Tier.None;


    /// <summary>
    /// For key characters that have their own music themes,
    /// this theme will play when the character enters.
    /// </summary>
    public EventReference keyMusicEvent;

}
