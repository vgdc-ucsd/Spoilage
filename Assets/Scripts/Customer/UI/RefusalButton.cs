using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// TODO: Once proper uses are created, remove customerOverride and the ternary
//       using it. This field was created only to keep the test scene running.

public class RefusalButton : MonoBehaviour
{
    public Animator anim;
    public UnityEvent buttonPress;
    [SerializeField] private GameObject guardsPrefab;
    [SerializeField] private GameObject guardSpawnpoint;
    [SerializeField] private GameObject customerOverride;
    public float guardMoveDuration = 1f;
    public float guardPauseAtCustomer = 0.5f;

    void Start()
    {
        buttonPress.AddListener(AnimateButton);
        buttonPress.AddListener(RemoveCustomer);
    }

    void OnMouseDown()
    {
        buttonPress.Invoke();
    }

    public void AnimateButton()
    {
        anim.SetTrigger("Button Pressed");
    }

    public void RemoveCustomer()
    {
        // TODO: Check ResourceManager for remaining refusals before proceeding.
        // If none left, play an error sound/animation and return.

        GameObject customerToRemove = customerOverride != null
            ? customerOverride
            : CustomerLineManager.Instance.CurrentCustomer?.gameObject;

        if (guardsPrefab != null && guardSpawnpoint != null && customerToRemove != null)
        {
            GameObject guards = Instantiate(guardsPrefab, guardSpawnpoint.transform.position, Quaternion.identity);
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
    }

    private IEnumerator GlideGuardToCustomerAndReturn(GameObject guards, GameObject customerToRemove)
    {
        Vector3 startPosition = guardSpawnpoint.transform.position;
        Vector3 customerPosition = customerToRemove.transform.position;

        float elapsed = 0f;
        while (elapsed < guardMoveDuration)
        {
            guards.transform.position = Vector3.Lerp(startPosition, customerPosition, elapsed / guardMoveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        guards.transform.position = customerPosition;
        yield return new WaitForSeconds(guardPauseAtCustomer);
        guards.GetComponent<Renderer>().material.color = Color.red;

        elapsed = 0f;
        while (elapsed < guardMoveDuration)
        {
            guards.transform.position = Vector3.Lerp(customerPosition, startPosition, elapsed / guardMoveDuration);
            customerToRemove.transform.SetParent(guards.transform);
            elapsed += Time.deltaTime;
            yield return null;
        }

        guards.transform.position = startPosition;

        // TODO: Call CustomerLineManager to advance to next customer or trigger end-of-section.
    }
}
