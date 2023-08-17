using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    public GameObject[] wheelsToRotate;

    public float rotationSpeed;

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        float verticalAxis = Input.GetAxisRaw("Vertical");
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        foreach (var wheel in wheelsToRotate)
        {
            wheel.transform.Rotate(Time.deltaTime * verticalAxis * rotationSpeed, 0, 0, Space.Self);
        }

        if (horizontalAxis > 0)
        {
            // going right
            anim.SetBool("goingLeft", false);
            anim.SetBool("goingRight", true);

        }
        else if (horizontalAxis < 0)
        {
            // going left
            anim.SetBool("goingLeft", true);
            anim.SetBool("goingRight", false);
        }
        else
        {
            // going straight
            anim.SetBool("goingLeft", false);
            anim.SetBool("goingRight", false);
        }
    }
}
