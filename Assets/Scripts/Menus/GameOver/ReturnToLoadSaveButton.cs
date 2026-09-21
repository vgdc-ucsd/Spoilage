using UnityEngine;

public class ReturnToLoadSaveButton : MonoBehaviour
{
    public void Click()
    {
        GameManager.Instance.Load(GameScene.LOAD_SAVE);
    }
}
