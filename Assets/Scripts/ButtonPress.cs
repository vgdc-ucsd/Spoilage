using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonPress : MonoBehaviour
{
    private int nextScene = 1;
    public void ClickedOn()
    { 
        SceneManager.LoadSceneAsync(nextScene);
    }

}
