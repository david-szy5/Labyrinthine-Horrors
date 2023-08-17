using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpeedBoost : MonoBehaviour
{
    public float boostFactor;

    void Start()
    {
        if (transform.gameObject.CompareTag("SpeedBoost_1"))
        {
            boostFactor = 1.25f;
        }
        else if (transform.gameObject.CompareTag("SpeedBoost_2"))
        {
            boostFactor = 1.75f;
        }
        else if (transform.gameObject.CompareTag("SpeedBoost_3"))
        {
            boostFactor = 2f;
        }
    }

    void Update() { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            var script = other.transform.parent.gameObject.GetComponent("CarController") as CarController;
            script.SpeedBoost(boostFactor);
            Destroy(transform.gameObject);
        }
    }
}
