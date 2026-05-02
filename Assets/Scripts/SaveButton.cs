using UnityEngine;

public class SaveButton : MonoBehaviour
{
    public void SaveState()
    {
        SaveSystem.SavePlayer();
    }
}
