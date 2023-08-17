using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideLocker : MonoBehaviour
{
    float detectionRange;
    bool isClose;
    Transform player;

    void Start()
    {
        player = GameObject.Find("PlayerFPS").transform;
        detectionRange = 5f;
    }

    void Update()
    {
        isClose = false;
        Debug.Log("isclose: " + isClose);
        if (Vector3.Distance(player.position, transform.position) <= detectionRange)
        {
            isClose = true;
        }
        if (isClose)
        {
            transform.LookAt(player.transform);
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
        if (isClose && Input.GetKeyUp(KeyCode.E))
        {
            //Hide logic
        }
    }

    void InteractGUI()
    {

    }
}
