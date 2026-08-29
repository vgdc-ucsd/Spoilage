using UnityEngine;
using UnityEngine.Events;

public class CallBell : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _callBellAction;
    private bool _locked;

    void Start()
    {
        _callBellAction.AddListener(CallBellStart);
    }

    public void CallBellStart()
    {
        _callBellAction.RemoveListener(CallBellStart);
    }

    public void Press()
    {
        if (_locked) return;
        _callBellAction.Invoke();
    }

    public void Lock(bool locked)
    {
        
    }
}
