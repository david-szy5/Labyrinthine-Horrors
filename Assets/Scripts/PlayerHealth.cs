using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Constants;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    // Player UI elements
    public float playerHealth;
    public float maxPlayerHealth;
    public uiManager uiManager;
    public HealthBar healthBar; 
    public bool gameOver = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        maxPlayerHealth = 100;
        playerHealth = maxPlayerHealth;
        //healthBar = GameObject.Find("Canvas").Find("HealthBar").GetComponent<HealthBar>();
    }

    
    
    public void TakeDamage(float amount)
    {
        Debug.Log("player taking damage");
        playerHealth -= amount;
        healthBar.UpdateHealthBar(amount);
        if (playerHealth <= 0)
        {
            // healthBar.transform.parent.gameObject.SetActive(false);
            if(gameOver==false) {
                Debug.Log("gameOver==false");
                gameOver = true;
                // StartCoroutine(GameObject.Find("Canvas").GetComponent<uiManager>().GameOverSequence(END_TYPE_LOSE));
                StartCoroutine(uiManager.GameOverSequence(END_TYPE_LOSE));
                
            }
        }
    }
    
    public void AddHealth(float amount) {
        if (playerHealth + amount > maxPlayerHealth) {
            amount = maxPlayerHealth - playerHealth;
        }
        playerHealth+=amount;
        healthBar.UpdateHealthBar(amount);
    }
}
    
