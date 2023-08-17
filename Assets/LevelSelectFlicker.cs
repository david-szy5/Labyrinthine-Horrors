using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectFlicker : MonoBehaviour
{
    private float flickerTime = 0.2f;
    private float timeInterval;
    private Light spotlight;
    private bool isFlickering = false;

    private void Awake()
    {
        spotlight = transform.GetComponent<Light>();
    }

    private void Update()
    {
        if (!isFlickering)
        {
            isFlickering = true;
            spotlight.enabled = false;
            timeInterval = Random.Range(0.1f, 3.0f);
            Invoke(nameof(resetLight), flickerTime);
        }
    }

    private void resetFlicker()
    {
        isFlickering = false;
    }

    private void resetLight()
    {
        spotlight.enabled = true;
        Invoke(nameof(resetFlicker), timeInterval);
    }
}
