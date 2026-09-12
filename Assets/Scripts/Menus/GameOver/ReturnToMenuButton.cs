using UnityEngine;

public class ReturnToMenuButton : MonoBehaviour
{
    public void Click()
    {
        GameManager.Instance.Load(GameScene.MAIN_MENU);
    }
}
