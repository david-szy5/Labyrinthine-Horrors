using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float maxTorque = 700f;
    public float maxSteerAngle = 60f;
    public WheelCollider[] wheels = new WheelCollider[4];
    public Transform[] wheelMesh = new Transform[4];
    public Transform t_CenterOfMass;

    float steer;
    Rigidbody r_Rigidbody;

    float brakeTorque;

    void Start()
    {
        r_Rigidbody = GetComponent<Rigidbody>();
        r_Rigidbody.centerOfMass = t_CenterOfMass.localPosition;

        int i = 0;
        foreach (Transform child in transform.Find("Wheels"))
        {
            wheels[i] = child.GetComponent<WheelCollider>();    
            i++;
        }
        i = 0;
        foreach (Transform child in transform.Find("CarMesh").Find("Wheels"))
        {
            wheelMesh[i] = child.transform;
            i++;
        }
    }

    public void Update()
    {
        RotateWheels();
    }

    void FixedUpdate()
    {
        steer = Input.GetAxis("Horizontal") * maxSteerAngle;
        float torque = Input.GetAxis("Vertical") * maxTorque;
        Rigidbody rb = GetComponent<Rigidbody>();

        wheels[0].steerAngle = steer;
        wheels[1].steerAngle = steer;

        foreach (WheelCollider wheel in wheels)
        {
            wheel.motorTorque = torque;
        }

        Vector3 velocity = rb.velocity;
        Vector3 orientation = transform.forward;
        float speed = Mathf.Abs(Vector3.Dot(velocity, orientation));
        Camera camera = transform.Find("MainCamera").GetComponent<Camera>();
        camera.fieldOfView = Mathf.Min(60 + speed / 1.5f, 150);
    }

    public void RotateWheels()
    {
        for (int i = 0; i < 4; i++)
        {
            //Quaternion quat;
            //Vector3 pos;

            //wheels[i].GetWorldPose(out pos, out quat);
            //wheelMesh[i].position = pos;
            //wheelMesh[i].transform.RotateAround(wheelMesh[i].transform., Vector3.up, steer);
        }
    }

    public void SpeedBoost(float factor)
    {
        int boost = 6;
        Rigidbody rb = GetComponent<Rigidbody>();

        IEnumerator DoBoost()
        {
            WaitForSeconds wait = new WaitForSeconds(0.025f);
            WaitForSeconds wait2 = new WaitForSeconds(0.5f);
            for (int i = 0; i < 10; i++)
            {
                rb.AddForce(i * boost * factor * transform.forward, ForceMode.Acceleration);
                yield return wait;
            }
            yield return wait2;
            for (int i = 0; i < 10; i++)
            {
                rb.AddForce(-i * boost * factor * transform.forward, ForceMode.Acceleration);
                yield return wait;
            }
        }

        StartCoroutine(DoBoost());
    }
}
