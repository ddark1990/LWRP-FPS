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
        PlayerVitals = PlayerVitals.Instance;
        InitStartingValues();
    }

    private void Update()
    {
        HealthBar.fillAmount = PlayerVitals.Health / startingMaxHealth;
        HealthRegenBar.fillAmount = PlayerVitals.Instance.desiredHealth / startingMaxHealth;
        HungerBar.fillAmount = PlayerVitals.Calories / startingMaxHunger;
        ThirstBar.fillAmount = PlayerVitals.Hydration / startingMaxThirst;

        HealthText.text = $"{(int) PlayerVitals.Health}";
        HungerText.text = $"{(int) PlayerVitals.Calories}";
        ThirstText.text = $"{(int) PlayerVitals.Hydration}";
    }

    private void InitStartingValues()
    {
        startingMaxHealth = PlayerVitals.maxHealth;
        startingMaxHunger = PlayerVitals.maxCalories;
        startingMaxThirst = PlayerVitals.maxHydration;
    }


}
