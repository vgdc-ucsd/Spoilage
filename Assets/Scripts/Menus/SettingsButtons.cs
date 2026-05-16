using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class SettingsButtons : MonoBehaviour
{
    public Slider Master;
    public GameObject BackButton;

    void Start()
    {
        if (Master != null)
            Master.GetComponent<Slider>().onValueChanged.AddListener(OnValueChanged);
        
        if (BackButton != null)
            BackButton.GetComponent<Button>().onClick.AddListener(Back);

        // Listen to changes

        float value = Master.value;
    }
    void Back()
    {
        Scene settings = SceneManager.GetSceneByName("Settings");
        if (settings.isLoaded)
        {
            SceneManager.UnloadSceneAsync("Settings");
            
            return;
        }
    }
    void OnValueChanged(float value)
    {
        AudioManager.Instance.SetVolume(value);
    }
    
}