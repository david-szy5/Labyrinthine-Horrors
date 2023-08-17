using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFireball : MonoBehaviour
{
    public float speed;
    public Space space;

    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * (speed * Time.deltaTime), space);
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }
}
