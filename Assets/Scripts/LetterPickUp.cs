using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterPickUp : MonoBehaviour
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    void OnTriggerEnter(Collider other) 
    {
        // ..and if the GameObject you intersect has the tag 'Pick Up' assigned to it..
        if (other.gameObject.CompareTag("Letter"))
        {
            other.gameObject.SetActive (false);
        }
    }

}