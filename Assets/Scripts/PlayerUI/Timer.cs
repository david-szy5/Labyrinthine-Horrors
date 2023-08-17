using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public float startTime = 15f; // seconds
    public float timeRemaining;
    public bool timerOn = false;
    public TMP_Text timerText;
    public TMP_FontAsset bangersFont;

    GameObject tempTime;

    void Start()
    {
        timerOn = true;
        timeRemaining = startTime;
    }

    void Update()
    {
        if (timerOn)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimer(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerOn = false;
                timerText.text = "0:00";
                timerText.color = new Color(255, 0, 0);
            }
        }
    }

    void UpdateTimer(float currentTime)
    {
        currentTime++;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        float elapsedTime = currentTime <= startTime ? (startTime - currentTime) : 0;
        float timeFraction = elapsedTime / startTime;

        timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);

        if (timeFraction >= 0.5)
        {
            timerText.color = new Color32(255, (byte)Mathf.FloorToInt((1.5f - timeFraction) * 255), (byte)Mathf.FloorToInt((1.5f - timeFraction) * 255), 255);
        }
        else
        {
            timerText.color = Color.white;
        }
    }

    public void TimerBoost(int factor)
    {
        int boost = 15 * factor;

        timeRemaining += boost;
        tempTime = new GameObject("TempTime");
        Canvas[] objs = FindObjectsOfType<Canvas>();
        
        foreach (Canvas obj in objs)
        {
            if (obj.gameObject.CompareTag("TimeCanvas"))
            {
                tempTime.transform.parent = obj.transform;
                break;
            }
        }

        tempTime.AddComponent<TextMeshProUGUI>();
        TMP_Text tempTimerText = transform.parent.Find("TempTime").gameObject.GetComponent<TextMeshProUGUI>();

        RectTransform rt = tempTimerText.gameObject.GetComponent<RectTransform>();
        rt.anchoredPosition3D = new Vector3(25, 255, 0);

        tempTimerText.text = "+" + boost.ToString();
        tempTimerText.fontSize = 28;
        tempTimerText.alignment = TextAlignmentOptions.Center;

        // FIX LATER
        //Font font = Resources.Load<Font>("Bangers-Regular.ttf"); 
        //TMP_FontAsset asset = TMP_FontAsset.CreateFontAsset(font);
        //tempTimerText.font = TMP_FontAsset.CreateFontAsset(Resources.GetBuiltinResource<Font>("Arial.ttf"));
        //tempTimerText.font = asset;

        IEnumerator DoFade()
        {
            WaitForSeconds wait = new WaitForSeconds(0.05f);
            for (int i = 1; i < 41; i++)
            {
                tempTimerText.color = new Color32(255, 255, 255, (byte)(255 - i * 6.375f));
                rt.anchoredPosition3D = new Vector3(25, 255 - 2 * i, 0);

                yield return wait;
            }
            Destroy(tempTime);
        }
        StartCoroutine(DoFade());
    }
}
