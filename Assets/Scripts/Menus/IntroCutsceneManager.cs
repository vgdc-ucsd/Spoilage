using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class IntroCutsceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (player) => AudioManager.Instance.PlayMusicEntry("IntroCutscene");
    }

    void Update()
    {
        if (videoPlayer.frame >= (long)videoPlayer.frameCount - 1)
        {
            OnVideoEnd();
        }
#if UNITY_EDITOR
        // Skip cutscene in editor for faster testing
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnVideoEnd();
        }
#endif
    }

    void OnVideoEnd()
    {
        GameManager.Instance.StartGame();
        Destroy(this);
    }
}
