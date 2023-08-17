using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    public PlayerHealth player;

    private void Awake()
    {
        //healthBar = transform.Find("HealthBarInner").GetComponent<Image>();
        //player = GameObject.Find("PlayerHolder").GetComponent<PlayerHealth>();
    }

    public void UpdateHealthBar(float amount)
    {
        float duration = 0.5f * (player.playerHealth / player.maxPlayerHealth);
        DOTween.To(() => healthBar.fillAmount, x => healthBar.fillAmount = x, player.playerHealth / player.maxPlayerHealth, duration);

        Color newColor = Color.green;
        if (player.playerHealth < player.maxPlayerHealth * 0.25f)
        {
            newColor = Color.red;
        }
        else if (player.playerHealth < player.maxPlayerHealth * 0.5f)
        {
            newColor = new Color(1f, 0.64f, 0f, 1f);
        }
        else if (player.playerHealth < player.maxPlayerHealth * 0.75f)
        {
            newColor = new Color(1f, 0.94f, 0, 1);
        }
        DOTween.To(() => healthBar.color, x => healthBar.color = x, newColor, duration * 3);
    }
}

