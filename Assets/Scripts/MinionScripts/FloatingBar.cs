using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingBar : MonoBehaviour
{
    private Slider slider;
    private float currentAmount, maxAmount;
    [SerializeField] private float updateSpeed = 0.001f;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }


    void Update()
    {
        if(slider.value < currentAmount / maxAmount)
        {
            slider.value += updateSpeed;
        }
        if(slider.value > currentAmount / maxAmount)
        {
            slider.value -= updateSpeed;
        }
    }

    public void UpdateHealthBar(float newHealth)
    {
        currentAmount = newHealth;
    }

    public void SetMaxHealthBar(float maxHealthBar)
    {
        maxAmount = maxHealthBar;
        currentAmount = maxAmount;
    }
}
