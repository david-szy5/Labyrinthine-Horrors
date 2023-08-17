using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCarController : MonoBehaviour
{
    // Get player input
    // move the sphere accordingly
    // make car model follow the sphere
    
    public Rigidbody sphereRB;
    public Rigidbody carRB;
    public float forwardSpeed; // controls how fast car goes
    public float reverseSpeed;
    public float turnSpeed;
    public LayerMask groundLayer;
    public float modifiedDrag;
    private float normalDrag;
    public float alignToGroundTime;
    public float gravForce;
    
    private float moveInput;
    private float turnInput;
    private bool isCarGrounded;

    public float playerHealth;
    public float maxPlayerHealth;
    public HealthBar healthBar; 

    // Start is called before the first frame update
    void Start()
    {
        sphereRB.transform.parent = null;
        carRB.transform.parent = null;
        normalDrag = sphereRB.drag;
        maxPlayerHealth = 100;
        playerHealth = 100;
        healthBar = transform.Find("Health").transform.Find("HealthBar").GetComponent<HealthBar>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Vertical");
        moveInput *= moveInput > 0 ? forwardSpeed : reverseSpeed;

        turnInput = Input.GetAxisRaw("Horizontal");

        // set car position to be equal to the sphere
        transform.position = sphereRB.transform.position;
        
        // rotate the car
        float newRotation = turnInput * turnSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical");
        if (isCarGrounded)
        {
            transform.Rotate(0, newRotation, 0, Space.World);
        }
        
        // check if we are grounded
        RaycastHit hit;
        isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);
        
        // rotate car to be parallel to the ground
        Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);

        sphereRB.drag = isCarGrounded ? normalDrag : modifiedDrag;

    }

    private void FixedUpdate()
    {
        if (isCarGrounded)
        {
            sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
        }
        else
        {
            sphereRB.AddForce(transform.up * -gravForce);
        }
        carRB.MoveRotation(transform.rotation);
    }

    public void TakeDamage(float amount)
    {
        playerHealth -= amount;
        healthBar.UpdateHealthBar(amount);

        if (playerHealth <= 0)
        {
            healthBar.transform.parent.gameObject.SetActive(false);
            // Implement lose scene
        }
    }
}
