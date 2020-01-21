using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class PlayerVitalsUI : MonoBehaviour
{
    public Image HealthBar;
    public Image HealthRegenBar;
    public Image HungerBar;
    public Image ThirstBar;
    public TextMeshProUGUI HungerText;
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI ThirstText;

    public PlayerVitals PlayerVitals;

    private float startingMaxHealth;
    private float startingMaxHunger;
    private float startingMaxThirst;


    private void Start()
    {
        InitStartingValues();
    }

    private void Update()
    {
        HealthBar.fillAmount = PlayerVitals.Health / startingMaxHealth;
        HealthRegenBar.fillAmount = PlayerVitals.Instance.desiredHealth / startingMaxHealth;
        HungerBar.fillAmount = PlayerVitals.Calories / startingMaxHunger;
        ThirstBar.fillAmount = PlayerVitals.Hydration / startingMaxThirst;

        HealthText.text = string.Format("{0}", (int)PlayerVitals.Health);
        HungerText.text = string.Format("{0}", (int)PlayerVitals.Calories);
        ThirstText.text = string.Format("{0}", (int)PlayerVitals.Hydration);
    }

    private void InitStartingValues()
    {
        startingMaxHealth = PlayerVitals.maxHealth;
        startingMaxHunger = PlayerVitals.maxCalories;
        startingMaxThirst = PlayerVitals.maxHydration;
    }


}
