using UnityEngine;

public class NextDayButton : MonoBehaviour
{
    public void NextDay()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        ProgressionManager.Instance.AdvanceDay();
    }
}
