using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectDamageEnemy : MonoBehaviour
{
    BasicEnemyController enemy;

    void Start()
    {
        enemy = transform.parent.Find("Player").GetComponent<BasicEnemyController>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            enemy.TakeDamage(10);
        }
    }
}
