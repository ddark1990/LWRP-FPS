using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemHolder : MonoBehaviour
{
    public Item CurrentlyHeldItem;
    public int ItemAmount;

    [Header("Cache")]
    public Image ItemImage;
    public Image ItemDurabilityBarImage;
    public Image ItemDurabilityBarBkgImage;
    public Text StackAmmountText;
    public Text GunAmmoAmountText;
    public Canvas canvas;

    public bool beingDragged;

    private int maxItemStackSize;

    private void Update()
    {
        UpdateItemUIInfo();
    }

    private void UpdateItemUIInfo()
    {
        UpdateItemDurabilityUI();
        UpdateGunAmmo();

        var tempSpriteColor = ItemImage.color;
        var tempTextColor = StackAmmountText.color;

        if (!CurrentlyHeldItem)
        {
            tempSpriteColor.a = 0;
            ItemImage.color = tempSpriteColor;

            tempTextColor.a = 0;
            StackAmmountText.color = tempTextColor;

            return;
        }

        maxItemStackSize = CurrentlyHeldItem.ItemType.itemData.StackLimit;

        tempSpriteColor.a = 255;
        ItemImage.color = tempSpriteColor;

        tempTextColor.a = 255;
        StackAmmountText.color = tempTextColor;

        ItemImage.sprite = CurrentlyHeldItem.ItemType.itemData.ItemSprite;

        if(ItemAmount == 1) //if weapon or something that is not stackable
        {
            StackAmmountText.text = string.Empty;

            return;
        }

        StackAmmountText.text = string.Format("x{0:n0}", ItemAmount);
    }

    private void UpdateItemDurabilityUI()
    {
        var tempDuraBarColor = ItemDurabilityBarImage.color; //alpha 255
        var tempDuraBarBkgColor = ItemDurabilityBarBkgImage.color; //alpha 75

        if (!CurrentlyHeldItem || !(CurrentlyHeldItem.ItemType is Weapon))
        {
            tempDuraBarColor.a = 0;
            tempDuraBarBkgColor.a = 0;

            ItemDurabilityBarImage.color = tempDuraBarColor;
            ItemDurabilityBarBkgImage.color = tempDuraBarBkgColor;

            return;
        }
        
        tempDuraBarColor.a = 255;
        ItemDurabilityBarImage.color = tempDuraBarColor;

        //tempDuraBarBkgColor.a = 75;
        //ItemDurabilityBarBkgImage.color = tempDuraBarBkgColor;

    }

    private void UpdateGunAmmo()
    {
        var tempGunAmmoAmountTextColor = GunAmmoAmountText.color; //alpha 255

        if (!CurrentlyHeldItem || !(CurrentlyHeldItem.ItemType is Weapon))
        {
            tempGunAmmoAmountTextColor.a = 0;

            GunAmmoAmountText.color = tempGunAmmoAmountTextColor;

            return;
        }

        var currentlyHoldingAmmo = CurrentlyHeldItem.GetComponent<GunReloadController>().currentHoldingAmmo;
        GunAmmoAmountText.text = currentlyHoldingAmmo.ToString();

        tempGunAmmoAmountTextColor.a = 255;
        GunAmmoAmountText.color = tempGunAmmoAmountTextColor;
    }
}
