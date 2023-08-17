using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{

    public Light myLight;
    public float maxTime;
    public float minTime;
    public float oldMin;
    public float oldMax;
    public float timer;
    public float audioTime;

    public bool hasAudio;
    public AudioSource AS;
    public AudioClip lightAudio;
    public GameObject EventSystem;

    private bool enabled;
    private bool flickering;
    private bool doOnce;

    void Start()
    {
        enabled = true;
        timer = Random.Range(minTime, maxTime);
        
        GameEvents.current.onTriggerFlickerLights += EventSetFlicker;
        GameEvents.current.onTriggerFlickerLights += IncreaseFlickerSpeed;
        
        GameEvents.current.onTriggerStopFlickerLights += DisableFlicker;
        GameEvents.current.onTriggerStopFlickerLights += ReturnFlickerSpeed;

        GameEvents.current.turnOffAllLights += TriggeredDisableLights;
        GameEvents.current.resetAllLights += TriggeredResetLights;

        
        flickering = false;
        doOnce = false;

    }

    void Update()
    { 
        
        if (!flickering && !doOnce)
        {
            float timeTilNextFlickering = Random.Range(5f, 6f);
            Invoke(nameof(EnableFlicker), timeTilNextFlickering);
            doOnce = true;
        }

        if (flickering && doOnce)
        {
            doOnce = false;
            float timeToDisable = Random.Range(1f, 3f);
            Invoke(nameof(DisableFlicker), timeToDisable);
        }

        if (flickering)
        {
            FlickerLight();
        }
        
    }

    void EnableFlicker()
    {
        flickering = true;
    }

    void IncreaseFlickerSpeed()
    {
        oldMin = minTime;
        oldMax = maxTime;
        
        minTime = 0.05f;
        maxTime = 0.1f;
    }

    void ReturnFlickerSpeed()
    {
        minTime = oldMin;
        maxTime = oldMax;
    }

    void DisableFlicker()
    {
        flickering = false;
        EnableLight();
    }

    void DisableLight()
    {
        myLight.enabled = false;
    }

    void EnableLight()
    {
        myLight.enabled = true;
    }

    void EventSetFlicker()
    {
        flickering = true;
        doOnce = false;
    }

    void TriggeredDisableLights()
    {
        DisableLight();
        flickering = false;
        doOnce = true;
    }

    void TriggeredResetLights()
    {
        EnableLight();
        flickering = true;
        doOnce = true;
    }

    private void FlickerLight()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            enabled = !enabled;
            myLight.enabled = enabled;
            if (hasAudio)
            {
                AS.clip = lightAudio;
                AS.time = 1f;
                AS.PlayScheduled(AudioSettings.dspTime);
                AS.SetScheduledEndTime(AudioSettings.dspTime + audioTime);
            }
            timer = Random.Range(minTime, maxTime);
        }
    }
}
