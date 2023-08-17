using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Script for under-barrel flashlight attachment
// The flashlight begins at maxBrightness and drains to minBrightness at the defined drainRate
// Player must turn off the light to recharge it
public class Flashlight : MonoBehaviour
{
    public float baseIntensity;
    public float maxBrightness;
    public float minBrightness;
    public float threshold;
    public float drainRate;
    public float capacity;
    public bool isFlickering;
    public Light flashlight;

    void Start()
    {
        flashlight = GetComponent<Light>();
        baseIntensity = 5000f;
        flashlight.intensity = baseIntensity;
        maxBrightness = 1f;
        minBrightness = 0.05f;
        drainRate = 10f;
        capacity = 1f;
        threshold = 0.05f; // Start lowering the brightness when flashlight reaches this capacity
        isFlickering = false;
    }

    void Update()
    {
        flashlight.intensity = Mathf.Clamp(flashlight.intensity, minBrightness * baseIntensity, maxBrightness * baseIntensity);
        if (flashlight.enabled || isFlickering)
        {
            capacity -= Time.deltaTime * (drainRate / 1000);
            if (capacity < threshold && capacity > minBrightness)
            {
                flashlight.intensity -= (Time.deltaTime * (drainRate / 5000) * baseIntensity);
                float randomTime = Random.Range(1f, 5f) / 10;
                flashlight.enabled = !flashlight.enabled;
                if (capacity <= 0.1f)
                {
                    randomTime += 0.35f;
                }
                isFlickering = true;
                FlickerTime(randomTime);
            }
        }
        else
        {
            capacity += 3 * Time.deltaTime * (drainRate / 1000);
            isFlickering = false;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;
        }
        if (capacity <= 0)
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }

    IEnumerator FlickerTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        flashlight.enabled = !flashlight.enabled;
    }
}

