using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    private Slider slider;
    private float health, maxHealth;
    [SerializeField] private float updateSpeed = 0.001f;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        if(slider.value < health / maxHealth)
        {
            slider.value += updateSpeed;
        }
        if(slider.value > health / maxHealth)
        {
            slider.value -= updateSpeed;
        }
    }

    public void UpdateHealthBar(float newHealth)
    {
        health = newHealth;
    }

    public void SetMaxHealthBar(float maxHealthBar)
    {
        maxHealth = maxHealthBar;
    }
}
