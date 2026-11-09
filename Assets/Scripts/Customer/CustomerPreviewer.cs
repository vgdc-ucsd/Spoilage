using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
public class CustomerPreviewer : MonoBehaviour
{
    private const int NumBases = 14;
    private const int NumEyes = 19;
    private const int NumMouths = 19;

    private int _baseIndex = 0;
    private int _eyesIndex = 1;
    private int _mouthIndex = 0;

    public GameObject _customerPrefab;
    public RectTransform _container;

    private List<CustomerAnimation> _animators = new();
    private bool _blink = false;

    private static readonly string[] s_eyesDirs =
    {
        "eyes_eyebrows100", "eyes_eyebrows101", "eyes_eyebrows102", "eyes_eyebrows103", "eyes_eyebrows104",
        "eyes_eyebrows167", "eyes_eyebrows248", "eyes_eyebrows255", "eyes_eyebrows257", "eyes_eyebrows258",
        "eyes_eyebrows259", "eyes_eyebrows260", "eyes_eyebrows261", "eyes_eyebrows277", "eyes_eyebrows283",
        "eyes_eyebrows286", "eyes_eyebrows288", "eyes_eyebrows292", "eyes_eyebrows332"
    };

    private static readonly string[] s_mouthDirs =
    {
        "nose_mouth100", "nose_mouth101", "nose_mouth102", "nose_mouth103", "nose_mouth104",
        "nose_mouth167", "nose_mouth248", "nose_mouth255", "nose_mouth257", "nose_mouth258",
        "nose_mouth259", "nose_mouth260", "nose_mouth261", "nose_mouth277", "nose_mouth283",
        "nose_mouth286", "nose_mouth288", "nose_mouth292", "nose_mouth332"
    };

    void Start()
    {
        // All bases

        // for (int i = 0; i < NumBases; i++)
        // {
        //     for (int j = 0; j < NumEyes; j++)
        //     {
        //         GameObject obj = Instantiate(_customerPrefab, _container);
        //         obj.name = s_eyesDirs[j];
        //         Customer customer = obj.GetComponent<Customer>();
        //         customer.customerData = CustomerManager.Instance.GenerateDebugCustomerData(i, j, 0);
        //         customer.InitializeCustomer();
        //     }

        //     for (int k = 0; k < NumMouths; k++)
        //     {
        //         GameObject obj = Instantiate(_customerPrefab, _container);
        //         obj.name = s_mouthDirs[k];
        //         Customer customer = obj.GetComponent<Customer>();
        //         customer.customerData = CustomerManager.Instance.GenerateDebugCustomerData(i, 0, k);
        //         customer.InitializeCustomer();
        //     }
        // }

        // Particular base

        for (int j = 0; j < NumEyes; j++)
        {
            GameObject obj = Instantiate(_customerPrefab, _container);
            obj.name = s_eyesDirs[j];
            Customer customer = obj.GetComponent<Customer>();
            customer.customerData = CustomerManager.Instance.GenerateDebugCustomerData(_baseIndex, j, 0);
            customer.InitializeCustomer();
            _animators.Add(obj.GetComponent<CustomerAnimation>());
        }

        for (int k = 0; k < NumMouths; k++)
        {
            GameObject obj = Instantiate(_customerPrefab, _container);
            obj.name = s_mouthDirs[k];
            Customer customer = obj.GetComponent<Customer>();
            customer.customerData = CustomerManager.Instance.GenerateDebugCustomerData(_baseIndex, 0, k);
            customer.InitializeCustomer();
            _animators.Add(obj.GetComponent<CustomerAnimation>());
        }

        // Particular base + eyes + mouth

        // GameObject obj = Instantiate(_customerPrefab, _container);
        // Customer customer = obj.GetComponent<Customer>();
        // customer.customerData = CustomerManager.Instance.GenerateDebugCustomerData(_baseIndex, _eyesIndex, _mouthIndex);
        // customer.InitializeCustomer();

        foreach (CustomerAnimation animator in _animators)
        {
            animator.SetBlinking(false);
        }
        Debug.Log(_animators.Count);
    }

    void Update()
    {
        // EYES

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            _blink = !_blink;
            Debug.Log($"Setting blink to {_blink}");
            foreach (CustomerAnimation animator in _animators)
            {
                animator.SetOpenEyes(_blink);
            }
        }

        // MOODS

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            Debug.Log("Setting mood to neutral");
            foreach (CustomerAnimation animator in _animators)
            {
                animator.SetMood(CustomerAnimation.Mood.NEUTRAL);
            }
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            Debug.Log("Setting mood to anger");
            foreach (CustomerAnimation animator in _animators)
            {
                animator.SetMood(CustomerAnimation.Mood.ANGER);
            }
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            Debug.Log("Setting mood to disgust");
            foreach (CustomerAnimation animator in _animators)
            {
                animator.SetMood(CustomerAnimation.Mood.DISGUST);
            }
        }
        else if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            Debug.Log("Setting mood to widen");
            foreach (CustomerAnimation animator in _animators)
            {
                animator.SetMood(CustomerAnimation.Mood.WIDENING);
            }
        }
    }
}
#endif
