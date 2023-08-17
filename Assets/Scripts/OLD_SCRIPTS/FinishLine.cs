using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static SendToGoogle;

public class FinishLine : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject endGameTextObject;
    public TextMeshProUGUI endGameText;

    private SendToGoogle sender;


    void Start()
    {
        endGameTextObject.SetActive(false);

        GameObject manager = GameObject.Find("GameManager");
        sender = manager.GetComponent<SendToGoogle>();
        
        Debug.Log(sender.sessionID);

        
        
    }

    void OnTriggerEnter(Collider other) 
    {
        Debug.Log("In OnTriggerEnter");
        // ..and if the GameObject you intersect has the tag 'Pick Up' assigned to it..
        if (other.gameObject.CompareTag("FinishLineCube"))
        {
            Debug.Log("FinishLine Detected");
            SetEndGameText();
        }

        sender.setEndType(1);
        sender.setCheckpoint(4);
        sender.setURL("https://docs.google.com/forms/u/0/d/e/1FAIpQLSeBZx3Kqt9QJ8PbwfhE2ht4mm7G_F0LtWbipezgfGKoec503A/formResponse");
        sender.Send();

        Debug.Log("At end of function");
    }

    void SetEndGameText() {
        endGameText.text = "Game Over";
        endGameTextObject.SetActive(true);
    }
}
