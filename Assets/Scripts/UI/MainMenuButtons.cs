using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void ClickStartGame()
    {
        //SceneManager.LoadSceneAsync("CustomerGeneration", LoadSceneMode.Additive);
        //SceneManager.LoadSceneAsync("CustomerInteraction", LoadSceneMode.Additive);
        SceneManager.LoadScene("Cooking");
    }

    public void ClickSettings()
    {
        
    }

    public void ClickExitGame()
    {
        // TODO - Save Game
        Application.Quit();
    }
}
