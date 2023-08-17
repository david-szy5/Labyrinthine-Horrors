using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{

    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action onTriggerFlickerLights;
    public event Action onTriggerStopFlickerLights;

    public event Action hidePlayer; // not sure if we need this one

    public event Action resetNoise;

    public event Action turnOffAllLights;
    public event Action resetAllLights;

    public event Action moveEnemyWaypoint;

    public event Action pauseTriggered;
    public event Action unpauseTriggered;

    private bool noiseMeterTriggered;
    private float timeToSafety;
    private float timeSpent;
    private bool enemyChasing;

    private void Start()
    {
        noiseMeterTriggered = false;
        timeToSafety = 5f;
        timeSpent = 0f;
        enemyChasing = false;
    }

    private void Update()
    {
        if (noiseMeterTriggered)
        {
            timeSpent += Time.deltaTime;
            if (timeSpent >= timeToSafety)
            {
                Debug.Log("In Danger! didn't hide in time!");
                // Spawn enemy near the player then turn off all the lights
                
                // spawn enemy
                SpawnEnemyNearPlayer();
                
                // turn off the lights
                TriggerLightsOff();
                
                // reset
                noiseMeterTriggered = false;
                timeSpent = 0f;
                enemyChasing = true;
            }
        }
    }

    public void TriggerPause()
    {
        pauseTriggered?.Invoke();
        Debug.Log("Paused");
        Time.timeScale = 0;
    }

    public void TriggerUnpause()
    {
        unpauseTriggered?.Invoke();
        Time.timeScale = 1;
    }

    public void SpawnEnemyNearPlayer()
    {
        moveEnemyWaypoint?.Invoke();
    }
    public void TriggerFlickerLights()
    {
        //Debug.Log("Flickering Lights Triggered");
        if (!enemyChasing)
            noiseMeterTriggered = true;
        
        onTriggerFlickerLights?.Invoke();
    }
    
    // This method should only be called once the player hides successfully (ie: not in line of sight of enemy)
    public void StopLightFlickering()
    {
        Debug.Log("Flickering Lights STOPPED");
        enemyChasing = false; // need to look into this logic
        noiseMeterTriggered = false;
        timeSpent = 0f;
        onTriggerStopFlickerLights?.Invoke();
        TriggerResetNoise();
    }

    // Resets the noise level of the player (done in the gun script)
    public void TriggerResetNoise()
    {
        Debug.Log("Noise Reset");
        resetNoise?.Invoke();
    }

    // Triggers all of the lights to go off before we spawn the enemy so that it is scary
    public void TriggerLightsOff()
    {
        Debug.Log("Lights OFF");
        turnOffAllLights?.Invoke();
        Invoke(nameof(TriggerResetLights), 10f);
    }

    // Resets all the lights back to their passive status, but then i'm going to set them to flicker til player hides
    public void TriggerResetLights()
    {
        Debug.Log("Lights back ON");
        resetAllLights?.Invoke();
        // TriggerFlickerLights();
    }

    public bool GetEnemyChaseStatus()
    {
        return enemyChasing;
    }
    
    
}
