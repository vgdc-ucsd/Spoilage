using UnityEngine;

[CreateAssetMenu(
    fileName = "SpoilageSymptom",
    menuName = "ScriptableObjects/SpoilageSymptom",
    order = 0
)]
public class Ourgh : AbstractSpoilageSymptom
{
    public Ourgh()
    {
        category = SpoilageCategory.DISGUST;
    }

    public override void ApplySpoilage() {
        Debug.Log("Ourgh");
        SetMood(CustomerAnimation.Mood.DISGUST);
        PlayDialogue("ourgh");
        // TODO: AUDIO
    }
}

