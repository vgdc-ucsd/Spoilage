using System;
using UnityEngine;
using UnityEngine.Events;

public class CallBell : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _callBellAction;

    void Start()
    {
        LockLayout.IsLocked = false;
        FoodGrab.CanMoveFood = false;

        _callBellAction.AddListener(CallBellStart);
    }

    public void CallBellStart()
    {
        LockLayout.IsLocked = true;
        FoodGrab.CanMoveFood = true;

        _callBellAction.RemoveListener(CallBellStart);
    }

    private void OnMouseDown()
    {
        _callBellAction.Invoke();
    }
}
