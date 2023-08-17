using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBoost : MonoBehaviour
{
    Timer timerScript;
    private int boostFactor;

    void Start()
    {
        timerScript = (Timer)FindObjectOfType(typeof(Timer));
    } 
    
    private void OnTriggerEnter(Collider other)
    {
        if (transform.gameObject.CompareTag("TimerBoost_1"))
        {
            boostFactor = 1;
        }
        else if (transform.gameObject.CompareTag("TimerBoost_2"))
        {
            boostFactor = 2;
        }
        else if (transform.gameObject.CompareTag("TimerBoost_3"))
        {
            boostFactor = 3;
        }

        if (other.CompareTag("Car"))
        {
            timerScript.TimerBoost(boostFactor);
            Destroy(transform.gameObject);
        }
    }
}
