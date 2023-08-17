using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasScript : MonoBehaviour
{
    public TMP_Text start_text;
    public TMP_Text controls;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DisableText", 5f);
    }

    // Update is called once per frame
    void DisableText() 
    {
        start_text.enabled = false;
        controls.enabled = false;
    }
}
