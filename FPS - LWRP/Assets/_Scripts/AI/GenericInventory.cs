using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericInventory : MonoBehaviour
{
    public List<GameObject> holdingItems;
    
    public bool holdingRangedWeapon;
    public bool holdingMeleeWeapon;
    
    public void AddItemToInventory(GameObject itemToAdd)
    {
        holdingItems.Add(itemToAdd);
        
        itemToAdd.transform.SetParent(transform);
        itemToAdd.SetActive(false);
    }

    public void RemoveItemFromInventory(GameObject itemToRemove)
    {
        itemToRemove.transform.SetParent(null);
        itemToRemove.SetActive(true);

        holdingItems.Remove(itemToRemove);
    }
    
    public bool HasWeaponsInInventory()
    {
        Item weapon = null;
        
        foreach (var item in holdingItems)
        {
            if (item.GetComponent<Item>().ItemType as Weapon)
            {
                weapon = item.GetComponent<Item>();
            }
        }
        return weapon != null && weapon.ItemType as Weapon;
    }

}
