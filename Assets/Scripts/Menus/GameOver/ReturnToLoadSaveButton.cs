using UnityEngine;

public class ReturnToLoadSaveButton : MonoBehaviour
{
    public void Click()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        GameManager.Instance.Load(GameScene.LOAD_SAVE);
    }
}
