using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Button _returnToMainMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _returnToMainMenu.onClick.AddListener(ReturnToMenu);
    }

    public void ReturnToMenu()
    {
        Debug.Log("Return to main menu");
        SceneManager.LoadScene("Menu");
    }
}
