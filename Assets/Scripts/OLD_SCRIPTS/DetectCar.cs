using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Mathematics.Random;

public class DetectCar: MonoBehaviour
{
    private Rigidbody myRigid;
    private BoxCollider myBox;
    private Unity.Mathematics.Random myRandom;
    public float fallDelayMin;
    public float fallDelayMax;

    private void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        myBox = GetComponent<BoxCollider>();
        myRandom = new Unity.Mathematics.Random(2);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Here...");
        if (other.CompareTag("car"))
        {
            Debug.Log("got here...");
            
            myBox.isTrigger = false;
            
            StartCoroutine(turnOnGravity());

        }
    }

    public IEnumerator turnOnGravity()
    {
        float mult = fallDelayMax - fallDelayMin;
        float fallDelay = fallDelayMin + mult * myRandom.NextFloat();
        yield return new WaitForSeconds(fallDelay);
        myRigid.isKinematic = false;
    }



}
