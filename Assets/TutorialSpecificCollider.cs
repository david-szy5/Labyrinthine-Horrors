using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialSpecificCollider : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text tutorial_text;
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("flashlight_trigger")) {
            tutorial_text.text = "Press F to use the flashlight to find the key";
            tutorial_text.enabled = true;
            Time.timeScale = 0;
            Destroy(other.gameObject);
        }
    }
}
