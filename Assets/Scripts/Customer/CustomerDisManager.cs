using UnityEngine;
using System.Collections.Generic;


public class CustomerDisManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //HELP
    Queue<GameObject> _customers = new Queue<GameObject>{};

    void Start()
    {
        //h
        //Either enqueue all customers from the start here and then pop when
        //necessary
    }

    // Update is called once per frame
    void Update()
    {
        //Or Enqueue as the customers disappear so that way we can keep the 
        //patience running and all that

    }

    void AddCustomer()
    {
        //We use the customer generations team's GenerateCustomer for a new customer
        //Then we control the said customers.
        //_customers.Enqueue(GenerateCustomer());
    }
}
