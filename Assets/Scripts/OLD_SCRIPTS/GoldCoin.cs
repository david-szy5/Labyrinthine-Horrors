using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    CoinDisplay coinScript;

    void Start()
    {
        coinScript = (CoinDisplay)FindObjectOfType(typeof(CoinDisplay));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            coinScript.AddCoin();
            Destroy(transform.gameObject);
        }
    }
}
