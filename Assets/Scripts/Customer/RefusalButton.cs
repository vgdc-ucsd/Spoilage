using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class RefusalButton : MonoBehaviour
{
    public Animator anim;
    public UnityEvent buttonPress;
    public GameObject guardsPrefab;
    public GameObject guardSpawnpoint;
    public GameObject customerToRemove;
    public float guardMoveDuration = 1f;
    public float guardPauseAtCustomer = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonPress.AddListener(AnimateButton);
        buttonPress.AddListener(RemoveCustomer);
        guardSpawnpoint = GameObject.FindGameObjectWithTag("Guard Spawnpoint");
        customerToRemove = GameObject.FindGameObjectWithTag("Customer Refusal Test");
    }


    void OnMouseDown()
    {
        Debug.Log("Button Pressed");
        buttonPress.Invoke();
    }

    public void AnimateButton()
    {
        anim.SetTrigger("Button Pressed");
    }

    public void RemoveCustomer()
    {
        if (guardsPrefab != null && guardSpawnpoint != null && customerToRemove != null)
        {
            GameObject guards = Instantiate(guardsPrefab, guardSpawnpoint.transform.position, Quaternion.identity);
            StartCoroutine(GlideGuardToCustomerAndReturn(guards));
        }
        else if (customerToRemove == null){
            Debug.Log("Customer to remove does not exist.");
        }
        else
        {
            Debug.LogError("One or more required objects are not assigned in the inspector.");
        }
    }

    private System.Collections.IEnumerator GlideGuardToCustomerAndReturn(GameObject guards)
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
        //Destroy(customerToRemove);
        guards.GetComponent<Renderer>().material.color = Color.red;

        elapsed = 0f;
        while (elapsed < guardMoveDuration)
        {
            guards.transform.position = Vector3.Lerp(customerPosition, startPosition, elapsed / guardMoveDuration);
            customerToRemove.transform.SetParent(guards.transform);
            //customerToRemove.transform.position = Vector3.Lerp(customerPosition, startPosition, elapsed / guardMoveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        guards.transform.position = startPosition;
    }
}
