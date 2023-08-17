using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyProjectileEnemy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player") || collision.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            Destroy(gameObject);
        }
    }
}
