using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RefusalButton : MonoBehaviour
{
    public Animator anim;
    public UnityEvent buttonPress;
    [SerializeField] private GameObject guardsPrefab;
    [SerializeField] private GameObject guardSpawnpoint;
    [SerializeField] private GameObject guardStaminaBar;
    private GuardsStaminaBar guardStaminaFillImage;
    public float guardMoveDuration = 1f;
    public float guardPauseAtCustomer = 0.5f;

    void Start()
    {
        buttonPress.AddListener(AnimateButton);
        buttonPress.AddListener(RemoveCustomer);
        guardStaminaBar = GameObject.FindGameObjectWithTag("Guard Stamina Bar");
        guardStaminaFillImage = guardStaminaBar.GetComponent<GuardsStaminaBar>();
    }

    public void Press()
    {
        buttonPress.Invoke();
    }

    public void AnimateButton()
    {
        if (anim != null)
        {
            anim.SetTrigger("Button Pressed");
        }
    }

    public void RemoveCustomer()
    {
        // TODO: Check ResourceManager for remaining refusals before proceeding.
        // If none left, play an error sound/animation and return.
        
        /* Customer currentCustomer = CustomerLineManager.Instance.CurrentCustomer;
        GameObject customerToRemove = currentCustomer?.gameObject;

        if (currentCustomer != null)
        {
            StoryManager.Instance.OnCustomerRefused(currentCustomer.customerData);
        }

        if (guardsPrefab != null && guardSpawnpoint != null && customerToRemove != null)
        {
            GameObject guards = Instantiate(guardsPrefab);
            StartCoroutine(GlideGuardToCustomerAndReturn(guards, customerToRemove));
        }
        else if (customerToRemove == null)
        {
            Debug.Log("No current customer to remove.");
        }
        else
        {
            Debug.LogError("One or more required objects are not assigned in the inspector.");
        }
        guardStaminaFillImage.buttonPressed(); */
    }

    private IEnumerator GlideGuardToCustomerAndReturn(GameObject guards, GameObject customerToRemove)
    {
        RectTransform guardRect = guards.GetComponent<RectTransform>();
        RectTransform spawnRect = guardSpawnpoint.GetComponent<RectTransform>();
        RectTransform customerRect = customerToRemove.GetComponent<RectTransform>();
        RectTransform animationParent = spawnRect.parent as RectTransform;

        Vector3 startPosition = spawnRect.position;
        Vector3 customerPosition = customerRect.position;

        guardRect.SetParent(animationParent, false);
        guardRect.position = startPosition;
        guardRect.SetAsLastSibling();

        float elapsed = 0f;
        while (elapsed < guardMoveDuration)
        {
            guardRect.position = Vector3.Lerp(startPosition, customerPosition, elapsed / guardMoveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        guardRect.position = customerPosition;
        yield return new WaitForSeconds(guardPauseAtCustomer);

        guards.GetComponent<Image>().color = Color.red;

        guardRect.SetAsLastSibling();

        elapsed = 0f;
        while (elapsed < guardMoveDuration)
        {
            Vector3 currentPosition = Vector3.Lerp(customerPosition, startPosition, elapsed / guardMoveDuration);
            guardRect.position = currentPosition;
            customerRect.position = currentPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        guardRect.position = startPosition;
        customerRect.position = startPosition;

        // CustomerLineManager.Instance.Advance();
    }
}
