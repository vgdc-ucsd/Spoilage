using UnityEngine;

public class ReturnToMenuButton : MonoBehaviour
{
    public void Click()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        GameManager.Instance.Load(GameScene.MAIN_MENU);
    }
}
