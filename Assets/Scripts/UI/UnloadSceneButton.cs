using UnityEngine;
using UnityEngine.UI;

public class UnloadSceneButton : MonoBehaviour
{
    private Button _button;
    /// <summary>
    /// This should be the name of the scene this button is in
    /// </summary>
    [SerializeField] private string _sceneName;

    void Start()
    {
        _button = gameObject.GetComponent<Button>();
        _button.onClick.AddListener(() => SceneLoader.Instance.UnloadScene(_sceneName));
    }

}
