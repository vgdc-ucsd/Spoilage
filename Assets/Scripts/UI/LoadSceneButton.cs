using UnityEngine;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    private Button _button;
    [SerializeField] private string _sceneName;

    void Start()
    {
        _button = gameObject.GetComponent<Button>();
        _button.onClick.AddListener(() => SceneLoader.Instance.ChangeScene(_sceneName));
    }

}
