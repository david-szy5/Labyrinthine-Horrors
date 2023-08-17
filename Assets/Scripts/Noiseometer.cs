using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Noiseometer : MonoBehaviour
{
    public GunSystem gs;

    public float maxNoise = 0.0f; // 
    public float minNoiseArrowAngle = 0.0f;
    public float maxNoiseArrowAngle = -180.0f;

    [Header("UI")]
    public Text noiseLabel; // The label that displays the noise;
    public RectTransform arrow; // The arrow in the speedometer

    private float noise = 0.0f;
    private void Start()
    {
        gs = GameObject.Find("Pistol").GetComponent<GunSystem>();
        maxNoise = gs.maxNoiseLevel;
        Debug.Log("max noise is " + maxNoise);
        
    }
    private void Update()
    {
        // 3.6f to convert in kilometers
        // ** The speed must be clamped by the car controller **
        noise = gs.GetNoiseLevel();

        // if (noiseLabel != null)
        //     noiseLabel.text = "Sound at " + ((int)noise) ;
        if (arrow != null) {
            //Debug.Log("in if");
            arrow.localEulerAngles =
                new Vector3(0, 0, Mathf.Lerp(minNoiseArrowAngle, maxNoiseArrowAngle, noise / maxNoise));
        }
    }
}
