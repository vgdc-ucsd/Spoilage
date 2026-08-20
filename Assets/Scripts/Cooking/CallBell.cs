using UnityEngine;
using UnityEngine.Events;

public class CallBell : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _callBellAction;

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
        _callBellAction.Invoke();
    }
}
