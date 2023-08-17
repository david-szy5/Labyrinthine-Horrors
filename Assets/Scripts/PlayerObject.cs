using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static Constants;

public class PlayerObject : MonoBehaviour
{
    public RawImage keyImage;
    public TMP_Text reloads_text;
    public TMP_Text key_text;

    public GameObject gun;
    private GunSystem gun_script;
    private PlayerHealth playerHealth;

    public GameObject enemy;

    public bool gameOver = false;
    private bool escaped = false;
    private bool hasKeys;
    private bool playerConcealed;
    private List<GameObject> nearVent = new List<GameObject>();

    Statistics stats;

    void Start()
    {
        hasKeys = false;
        gun_script = gun.GetComponent<GunSystem>();
        stats = GameObject.Find("GameManager").GetComponent<Statistics>();
        playerHealth = GameObject.Find("PlayerHolder").GetComponent<PlayerHealth>();
        playerConcealed = false;
    }

    public bool hasKey() {
        return hasKeys;
    }

    public bool isPlayerHidden()
    {
        return playerConcealed;
    }

    public bool isPlayerNearVent()
    {
        return nearVent.Count > 0;
    }

    public void setNearVent(GameObject vent)
    {
        nearVent.Add(vent);
    }

    public void removeNearVent(GameObject vent)
    {
        nearVent.Remove(vent);
    }

    public List<GameObject> getListOfVents()
    {
        return nearVent;
    }

    public void setPlayerConcealment(bool playerHidden)
    {
        playerConcealed = playerHidden;

        if (playerHidden)
        {
            GameEvents.current.StopLightFlickering();
            if (enemy != null)
            {
                NewSeekerEnemyAI enemyAI = enemy.GetComponent<NewSeekerEnemyAI>();
                enemyAI.moveToNearestWayPoint();
            }
        }
        else
        {
            if (enemy != null)
            {
                NewSeekerEnemyAI enemyAI = enemy.GetComponent<NewSeekerEnemyAI>();
                enemyAI.seekerMode = enemyAI.SCOUT_MODE;
            }
        }
    }


    private void UpdateKeyUI()
    {
        hasKeys = true;
        keyImage.color = new Color32(255, 255, 255, 255);
        key_text.text = "1/1";
    }

    private void OnCollisionEnter(Collision other) {
        
        if (other.collider.gameObject.CompareTag("seeker")) {
            StartCoroutine(GameObject.Find("Canvas 2").GetComponent<uiManager>().GameOverSequence(END_TYPE_LOSE));
            // StartCoroutine(uiManager.GameOverSequence(END_TYPE_LOSE));
        }
        if (other.collider.gameObject.CompareTag("BulletEnemy")) {
            //StartCoroutine(GameObject.Find("Canvas").GetComponent<uiManager>().GameOverSequence(END_TYPE_LOSE));
            playerHealth.TakeDamage(10);
        }
    }

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("key")) {
            UpdateKeyUI();
            stats.findKey();
            Destroy(other.gameObject);
        } else if (other.CompareTag("ammo") && gun_script.numReloadsLeft != gun_script.numReloads) {
            gun_script.AddMagazine();
            Destroy(other.gameObject);
            stats.pickup();
            
        } else if (other.CompareTag("escape") && hasKey() && !escaped) {
            GameObject escapeDoor = GameObject.Find("EscapeDoor");
            escapeDoor.transform.Translate(-10, 0, 0);
            escaped = true;
            //not sure if this is end condition we want?
            Debug.Log("Through the door!");
            StartCoroutine(GameObject.Find("Canvas 2").GetComponent<uiManager>().GameOverSequence(END_TYPE_WIN));
            // StartCoroutine(uiManager.GameOverSequence(END_TYPE_WIN));
        }
        else if (other.CompareTag("escape") && !hasKey()) {
            stats.gateCollide();
        }
        else if (other.CompareTag("health")) {
            if (playerHealth.playerHealth != playerHealth.maxPlayerHealth) {
                    Debug.Log(playerHealth.playerHealth + " " + playerHealth.maxPlayerHealth);
                    playerHealth.AddHealth(10);
                    Destroy(other.gameObject);
            }
        }
    }
}
