using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Item[] listOfWeapons;

    public void ToggleActiveWeapon(Weapon equipedWeapon, bool state)
    {
        for (int i = 0; i < listOfWeapons.Length; i++)
        {
            var weapon = listOfWeapons[i];

            if (weapon.ItemType.itemData.ItemName == equipedWeapon.itemData.ItemName)
            {
                weapon.gameObject.SetActive(state);
            }
        }
    }
    
}
