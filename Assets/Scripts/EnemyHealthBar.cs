using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyHealthBar : MonoBehaviour
{
    public Image healthBar;
    public BasicEnemyController enemy;
    public float tweenSpeed = 0.5f;
  

    private void Awake()
    {
        healthBar = transform.parent.Find("HealthBarInner").GetComponent<Image>();
       // Debug.Log(healthBar);
        enemy = transform.parent.parent.GetComponent<BasicEnemyController>();
        //Debug.Log(enemy);
    }

    void Update()
    {
        transform.parent.LookAt(GameObject.Find("PlayerHolder").transform);
    }

    public float UpdateHealthBar(float amount)
    {
        if (enemy != null)
        {
            float duration = tweenSpeed * (enemy.health / enemy.maxHealth);
            DOTween.To(() => healthBar.fillAmount, x => healthBar.fillAmount = x, enemy.health / enemy.maxHealth, duration);
            return duration;
        }

        return 0;
    }
}
