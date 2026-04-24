using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    [SerializeField] private Slider _volumeSlider;

    void Start()
    {

    }

    public void ChangeVolume()
    {
        AudioListener.volume = _volumeSlider.value;
    }
}
