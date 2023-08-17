using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    PickUpDisplay pickupScript;

    void Start()
    {
        pickupScript = (PickUpDisplay)FindObjectOfType(typeof(PickUpDisplay));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            pickupScript.SubtractCount();
        }
    }
}
