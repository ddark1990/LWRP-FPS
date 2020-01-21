using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelUIController : MonoBehaviour
{
    public GameObject ActivePanel;

    [Header("Panels")]
    public GameObject BackInfoPanel;
    public GameObject SplittingPanel;
    public GameObject EatFoodPanel;

    [Header("General")]
    public Text ItemNameText;

    [Header("Eat Food Panel Cache")]
    public Image EatFoodItemSprite;
    public Text EatFoodItemInfoText;
    public Text EatFoodHealingAmountText;
    public Text EatFoodCaloriesAmountText;
    public Text EatFoodHydrationAmountText;
    public Button EatFoodButton;
    public Button DropFoodButton;

    private InventoryManager invMan;

    private void Start()
    {
        invMan = InventoryManager.Instance;
        ResetAllPanels();
        InvokeRepeating("UpdateEatFoodButton", 0.1f, 0.1f);
    }

    private void Update()
    {
        //ChangeActivePanel();
        if (Input.GetKeyDown(InputManager.Instance.InputKeyManager.OpenInventoryKey) && !InventoryManager.Instance.MenuOpen)
            ResetAllPanels();
    }

    public void ChangeActivePanel()
    {
        if (!invMan.CurrentlySelectedSlot) return;

        var curHeldItem = invMan.CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem;

        if (!curHeldItem) return;

        if (curHeldItem.ItemType is Consumable)
        {
            BackInfoPanel.SetActive(true);
            ActivePanel = EatFoodPanel;
            EatFoodPanel.SetActive(true);

            Debug.Log("Opening Consumable Type Info Panel");
        }
        else if(curHeldItem.ItemType is Weapon)
        {

        }
        else ResetAllPanels();


        PopulatePanelData(curHeldItem.ItemType, curHeldItem);
    }

    private void PopulatePanelData(ItemType itemType, Item curHeldItem)
    {
        if(itemType is Consumable)
        {
            var consumableType = itemType as Consumable;

            ItemNameText.text = curHeldItem.ItemType.itemData.ItemName;
            EatFoodItemSprite.sprite = curHeldItem.ItemType.itemData.ItemSprite;
            EatFoodItemInfoText.text = curHeldItem.ItemType.itemData.ItemDescription;
            EatFoodHealingAmountText.text = "+" + consumableType.consumableSettings.GiveHealthAmount.ToString();
            EatFoodCaloriesAmountText.text = "+" + consumableType.consumableSettings.GiveCaloriesAmount.ToString();
            EatFoodHydrationAmountText.text = "+" + consumableType.consumableSettings.GiveHydrationAmount.ToString();

            Debug.Log("Populating Consumable Type Info Panel");
        }
        else if (itemType is Weapon)
        {
            var weaponType = itemType as Weapon;

            //wepaon data
        }
    }

    public void ResetAllPanels()
    {
        //Debug.Log("Reseting All Panels!");

        ActivePanel = null;

        BackInfoPanel.SetActive(false);
        EatFoodPanel.SetActive(false);
    }

    private void UpdateEatFoodButton()
    {
        if (PlayerVitals.Instance.active) EatFoodButton.enabled = false;
        else EatFoodButton.enabled = true;
    }
}
