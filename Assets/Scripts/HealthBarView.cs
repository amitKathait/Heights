using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private int maxHealth = 100;
    
    
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
       SetSliderValue();
    }
    
    public void TakeDamage(int damage, Action callback)
    {
        currentHealth -= damage;
        SetSliderValue();
        if (currentHealth <= 0)
        {
            callback?.Invoke();
        }
    }

    private void SetSliderValue()
    {
        slider.value = currentHealth;
    }
}
