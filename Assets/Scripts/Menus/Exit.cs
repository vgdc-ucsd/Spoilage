using UnityEngine;

public class Exit : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Exiting Application");
        Application.Quit();
    }
}
