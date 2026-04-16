using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    private int _nextScene = 1;
    public void ClickStartGame()
    { 
        SceneManager.LoadSceneAsync(_nextScene);
    }

}
