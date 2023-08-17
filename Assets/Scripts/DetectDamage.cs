using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectDamage : MonoBehaviour
{
    NewCarController player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<NewCarController>();
        Physics.IgnoreCollision(GameObject.Find("ArenaTester").transform.Find("Plane").GetComponent<MeshCollider>(), GetComponent<BoxCollider>());
        Debug.Log("start player: " + player);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            player.TakeDamage(10);
        }
    }
}
