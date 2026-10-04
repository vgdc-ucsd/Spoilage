using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RefusalButton : MonoBehaviour
{
    [SerializeField] private Sprite _buttonUnpressed;
    [SerializeField] private Sprite _buttonPressed;
    [SerializeField] private Image _button;
    private bool _locked;

    void Start()
    {
        _button.sprite = _buttonUnpressed;
    }

    public void Press()
    {
        if (_locked) return;
        if (!GuardManager.Instance.RemoveCustomer()) return;
        StopAllCoroutines();
        StartCoroutine(PressButtonAnim());
    }

    public void Lock(bool locked)
    {
        _locked = locked;
    }

    private IEnumerator PressButtonAnim()
    {
        _button.sprite = _buttonPressed;
        yield return new WaitForSeconds(0.1f);
        _button.sprite = _buttonUnpressed;
    }
}
