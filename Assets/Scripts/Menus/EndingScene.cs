using UnityEngine;
using UnityEngine.UI; 

public class EndingCutscene : MonoBehaviour
{
    public Sprite finalCutscene;
    private Image cutsceneImage;
    void Start()
    {
        cutsceneImage = GetComponent<Image>();
        cutsceneImage.sprite = finalCutscene;
        Debug.Log("Ending cutscene started.");
    }
}
