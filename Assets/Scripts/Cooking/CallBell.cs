using UnityEngine;

public class CallBell : MonoBehaviour
{
    private bool _locked;

    public void Press()
    {
        if (_locked) return;
        CookingManager.Instance.SubmitOrder();
    }

    public void Lock(bool locked)
    {
        _locked = locked;
    }
}
