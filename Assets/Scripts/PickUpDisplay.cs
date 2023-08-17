using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickUpDisplay : MonoBehaviour
{
    public TMP_Text pickupText;
    int count;

    void Start()
    {
        //displayCoins = true;
        count = 5;
        pickupText.text = "Items left: " + count.ToString();
    }

    public void SubtractCount()
    {
        count-=1;
        pickupText.text = "Items left: " + count.ToString();

    }
}