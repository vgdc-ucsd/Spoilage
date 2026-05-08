using UnityEngine;
using UnityEngine.UI; 
public class SettingsButtons : MonoBehaviour
{
    public Slider Master;
    

    void Start()
    {
        Master = GameObject.Find("Master").GetComponent<Slider>();
        float value = Master.value;
        
        // Listen to changes
        Master.onValueChanged.AddListener(OnValueChanged);
    }
    
    void OnValueChanged(float value)
    {
        AudioManager.Instance.SetVolume(value);
    }
   
}