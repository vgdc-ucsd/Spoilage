using UnityEngine;
using UnityEngine.Video;

public class IntroCutsceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    void Update()
    {
        if (videoPlayer.frame >= (long)videoPlayer.frameCount - 1)
        {
            OnVideoEnd();
        }
    }

    void OnVideoEnd()
    {
        Debug.Log("Video ended");
        GameManager.Instance.StartGame();
        Destroy(this);
    }
}
