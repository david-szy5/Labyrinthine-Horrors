using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tutorial : MonoBehaviour
{

    public TMP_Text crouchControls;
    public TMP_Text tutorialText;
    public GameObject noisemeter_text;
    bool isPaused = true;
    public float sightRange;
    public Transform player;
    public Transform enemyOne;
    public GameObject enemy1;
    public GameObject healthPickUp;
    public GameObject ammoPickUp;
    private GunSystem gs;
    public bool pickupMessageSeen = false;
    bool enemyOneEncountered = false;
    public int numEPressed = 0;
    public bool ePressed = false;
    public bool cPressed = false;
    private bool noiseTextShown = false;

    void Start()
    {
        GameEvents.current.TriggerPause();
        tutorialText.enabled = true;
        // doorControl.enabled = false;
        crouchControls.enabled = false;
        // enemyOneInfo.enabled = false;
        // pickupMessage.enabled = false;
        noisemeter_text.SetActive(false);
        enemy1.SetActive(false);
        gs = GameObject.Find("Pistol").GetComponent<GunSystem>();
        // GameEvents.current.unpauseTriggered += UnpauseGame;
    }

    void Update()
    {   
        float distance = sightRange + 1;
        if (enemyOne != null) {
            distance = Vector3.Distance(player.position, enemyOne.position);
        }
        if (isPaused && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && tutorialText.enabled && !pickupMessageSeen)
        {
            UnpauseGame();
            crouchControls.enabled = true;
        } else if (Input.GetKeyDown(KeyCode.E) && enemy1!=null) {
            ePressed = true;
            numEPressed += 1;
            if (numEPressed == 2) {
                enemy1.SetActive(true);
            }
        } else if (isPaused && pickupMessageSeen && tutorialText.enabled && (healthPickUp == null || ammoPickUp == null)) {
            UnpauseGame();
        } else if (enemy1 == null && !pickupMessageSeen) {
            healthPickUp.SetActive(true);
            ammoPickUp.SetActive(true);
            tutorialText.text = "Pick up the ammo object to replenish bullets." + '\n' + '\n' + "Pick up the + object to gain +10 health";
            tutorialText.enabled = true;
            isPaused = true;
            pickupMessageSeen = true;
        } else if (distance <= sightRange && !isPaused && !enemyOneEncountered) {
            tutorialText.text = "Click to shoot the enemy";
            tutorialText.enabled = true;
            enemyOneEncountered = true;
            GameEvents.current.TriggerPause();
            isPaused = true;
        } else if (enemyOneEncountered && Input.GetMouseButtonDown(0) && !isPaused && gs.GetNoiseLevel() > 0 && !noiseTextShown) {
            Debug.Log("in loop");
            noisemeter_text.SetActive(true);
            noiseTextShown = true;
            GameEvents.current.TriggerPause();
            isPaused = true;
        }
        else if (Input.GetMouseButtonDown(0) && isPaused && enemyOneEncountered && !pickupMessageSeen) {
            UnpauseGame();
        } else if (tutorialText.enabled && tutorialText.text == "Press F to use the flashlight to find the key" && Input.GetKeyDown(KeyCode.F)) {
            UnpauseGame();
        }
    }

    void UnpauseGame()
    {
        GameEvents.current.TriggerUnpause();
        tutorialText.enabled = false;
        noisemeter_text.SetActive(false);
        isPaused = false;
    }
    
}