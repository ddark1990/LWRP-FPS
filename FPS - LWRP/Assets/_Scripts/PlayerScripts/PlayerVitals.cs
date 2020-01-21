using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVitals : MonoBehaviour
{
    public static PlayerVitals Instance;

    public float Health;
    public float Calories;
    public float Hydration;

    [Range(0, 25)] public float hungerRate;
    [Range(0, 25)] public float thirstRate;
    public float healthFromFoodRegen;
    public float intervalBetweenEating;

    [HideInInspector] public float maxHealth = 100;
    [HideInInspector] public float maxCalories = 500;
    [HideInInspector] public float maxHydration = 250;
    public float activeTimer;
    public bool active;

    private InventoryManager inventoryManager;

    public bool generate;
    public float desiredHealth;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        inventoryManager = FindObjectOfType<InventoryManager>();

        StartStats();
    }

    void Update()
    {
        HungerOutput();
        ThirstOutput();

        HealthClamp();
    }

    private void StartStats()
    {
        Health = Random.Range(50, 60);
        Calories = Random.Range(100, 110);
        Hydration = Random.Range(100, 110);
    }

    private void HungerOutput()
    {
        Calories = Mathf.Clamp(Calories, 0, maxCalories);
        Calories -= Time.deltaTime * hungerRate;
    }
    private void ThirstOutput()
    {
        Hydration = Mathf.Clamp(Hydration, 0, maxHydration);
        Hydration -= Time.deltaTime * thirstRate;
    }

    private void HealthClamp()
    {
        Health = Mathf.Clamp(Health, 0, maxHealth);
    }


    public void OnEatFoodPress()
    {
        StartCoroutine(EatFood(intervalBetweenEating));

        EatFoodLogic(inventoryManager);
    }
    private IEnumerator EatFood(float waitTime)
    {
        activeTimer = 0f;

        while (activeTimer < waitTime)
        {
            activeTimer += Time.deltaTime;

            yield return null;

            if (activeTimer >= waitTime)
            {
                //Debug.Log("ACTIVE TIMER OVER");
                active = false;

                yield break;
            }
        }
    }
    private void EatFoodLogic(InventoryManager invMan)
    {
        if (active) return;

        active = true;
        invMan.UseSlot.ItemHolder.ItemAmount -= 1;

        ////
        invMan.UpdateItemInsideInventory(invMan.UseSlot.ItemHolder.CurrentlyHeldItem, invMan.InventorySlots, invMan.HotBarSlots); //i dunno bro
        ////

        invMan.audioSource.PlayOneShot(invMan.UseSlot.ItemHolder.CurrentlyHeldItem.ItemType.itemSoundData.ActivateItemSound, invMan.uiVolume);

        RegenHealth(inventoryManager);

        var consumableType = invMan.UseSlot.ItemHolder.CurrentlyHeldItem.ItemType as Consumable;
        Calories += consumableType.consumableSettings.GiveCaloriesAmount;
        Hydration += consumableType.consumableSettings.GiveHydrationAmount;

        if (invMan.UseSlot.ItemHolder.ItemAmount.Equals(0))
        {
            invMan.ResetUseSlot();
        }
    }


    private void RegenHealth(InventoryManager invMan) 
    {
        if (Health.Equals(maxHealth)) return;

        var consumableType = invMan.UseSlot.ItemHolder.CurrentlyHeldItem.ItemType as Consumable;
        var itemHealthAmount = consumableType.consumableSettings.GiveHealthAmount;

        if (desiredHealth == 0)
            desiredHealth = Instance.Health + itemHealthAmount;
        else
            desiredHealth += itemHealthAmount;

        //Debug.Log(desiredHealth);

        if (!generate)
        {
            generate = true;
            StartCoroutine(RegenerateHealth(healthFromFoodRegen));
        }
    }
    private IEnumerator RegenerateHealth(float time)
    {
        while (generate)
        {
            Instance.Health += time * Time.deltaTime;

            if (desiredHealth > Instance.maxHealth)
                desiredHealth = Instance.maxHealth;

            desiredHealth = Mathf.Clamp(desiredHealth, 0, Instance.maxHealth);

            yield return null;

            if (Instance.Health >= desiredHealth)
            {
                //Debug.Log("DONE GENERATING HEALTH");
                desiredHealth = 0;
                generate = false;
                yield break;
            }
        }
    }

}
