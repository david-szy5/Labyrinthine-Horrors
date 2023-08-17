using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballShot : MonoBehaviour
{
    private float shootTimer; // 4
    public float timeBetweenShots;

    public int numFireBalls;
    public Transform fireSpawnpoint; // 2
    public GameObject fireballPrefab;
    private Vector3 direction;

    private Queue<GameObject> fireballQueue = new Queue<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        direction = fireSpawnpoint.position - transform.position;
        StartCoroutine(ShootFireballCoroutine());

    }

    // Update is called once per frame
    void Update()
    {
        // direction = fireSpawnpoint.position - transform.position;
        
        // transform.Translate(movementSpeed * Time.deltaTime, space);
    }

    private IEnumerator ShootFireballCoroutine()
    {
        while (true)
        {
            ShootFireball();
            yield return new WaitForSeconds(timeBetweenShots);
        }
        
    }

    private void ShootFireball()
    {
        GameObject fireball = Instantiate(fireballPrefab, fireSpawnpoint.position, Quaternion.identity);
        fireballQueue.Enqueue(fireball);
        if (fireballQueue.Count > numFireBalls)
        {
            Destroy(fireballQueue.Dequeue());
        }
        MoveFireball moveFireball = fireball.GetComponent<MoveFireball>();
        moveFireball.SetDirection(direction);
    }
}
