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
            Debug.Log("Skipping cutscene...");
            OnVideoEnd();
        }
#endif
    }

    void OnVideoEnd()
    {
        Debug.Log("Video ended");
        GameManager.Instance.StartGame();
        Destroy(this);
    }
}
