using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    public Vector3 centerOfMass;
    public Rigidbody rb;

    void Start()
    {
        rb = transform.parent.parent.GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;
    }
}
