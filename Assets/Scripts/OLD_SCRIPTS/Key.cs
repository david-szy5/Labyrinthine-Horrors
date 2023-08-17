using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    void Start() { }

    void Update() { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("car"))
        {
            var script = GameObject.Find("NewCar").GetComponent("NewCarController") as NewCarController;
            //script.changeKeyValue();
            transform.parent.Find("Door").GetComponent<Collider>().isTrigger = true;
            Destroy(transform.parent.Find("Cube").gameObject);
            Destroy(transform.parent.Find("Cube (1)").gameObject);
            Destroy(transform.parent.Find("Cube (2)").gameObject);
            Destroy(transform.gameObject);
        }
    }
}
