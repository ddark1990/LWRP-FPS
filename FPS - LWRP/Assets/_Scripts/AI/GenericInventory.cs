using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericInventory : MonoBehaviour
{
    public List<Item> holdingItems;
    
    public void AddItemToInventory(Item itemToAdd)
    {
        holdingItems.Add(itemToAdd);
        
        itemToAdd.transform.SetParent(transform);
        itemToAdd.transform.localPosition = new Vector3(0,0,0);
        itemToAdd.gameObject.SetActive(false);
    }

    public void RemoveItemFromInventory(Item itemToRemove)
    {
        itemToRemove.transform.SetParent(null);
        itemToRemove.gameObject.SetActive(true);

        holdingItems.Remove(itemToRemove);
    }
    
    public bool HasWeaponInInventory()
    {
        Item weapon = null;
        
        foreach (var item in holdingItems)
        {
            if (item.ItemType as Weapon)
            {
                weapon = item;
            }
        }
        return weapon != null && weapon.ItemType as Weapon;
    }

    public Weapon GetBestWeaponFromInventory()
    {
        Weapon bestWeapon = null;

        for (var i = 0; i < holdingItems.Count; i++)
        {
            var itemType = holdingItems[i].ItemType;

            if (!(itemType as Weapon)) continue;
            
            bestWeapon = itemType as Weapon;

            if (bestWeapon != null && ((Weapon) itemType).weaponSettings.weaponTier > bestWeapon.weaponSettings.weaponTier)
            {
                bestWeapon = (Weapon)itemType;
            }
        }
        
        return bestWeapon;
    }
}
