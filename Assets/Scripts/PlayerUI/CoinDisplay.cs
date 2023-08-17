using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    public TMP_Text coinText;
    //bool displayCoins;
    int numCoins;
    GameObject tempCoin;

    void Start()
    {
        //displayCoins = true;
        numCoins = 0;
        coinText.text = numCoins.ToString();
    }

    public void AddCoin()
    {
        numCoins++;
        coinText.text = numCoins.ToString();

        tempCoin = new GameObject("TempCoin");
        Canvas[] objs = FindObjectsOfType<Canvas>();

        foreach (Canvas obj in objs)
        {
            if (obj.gameObject.CompareTag("GoldCanvas"))
            {
                tempCoin.transform.parent = obj.transform;
                break;
            }
        }

        tempCoin.AddComponent<TextMeshProUGUI>();
        TMP_Text tempCoinText = transform.parent.Find("TempCoin").gameObject.GetComponent<TextMeshProUGUI>();

        RectTransform rt = tempCoinText.gameObject.GetComponent<RectTransform>();
        rt.anchoredPosition3D = new Vector3(605, 240, 0);

        tempCoinText.text = "+1";
        tempCoinText.fontSize = 28;

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
                tempCoinText.color = new Color32(210, 170, 30, (byte)(255 - i * 6.375f));
                rt.anchoredPosition3D = new Vector3(605, 240 - i, 0);

                yield return wait;
            }
            Destroy(tempCoin);
        }
        StartCoroutine(DoFade());
    }
}
