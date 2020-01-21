using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HotbarController : MonoBehaviour
{
    private int selectedHotbarIndex;

    public InventoryManager inventoryManager;
    public WeaponManager weaponManager;
    public ItemPlacementSystem itemPlacement;

    private KeyCode[] hotBarControls = new KeyCode[]
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6
    };

    void Update()
    {
        GetHotBarInput();
    }

    private void GetHotBarInput()
    {
        //UpdateSelectedHotBarIndex(Input.GetAxis("Mouse ScrollWheel")); //fix later

        if (!Input.anyKey) return;

        for (int i = 0; i < hotBarControls.Length; i++)
        {
            if(Input.GetKeyDown(hotBarControls[i]))
            {
                if (inventoryManager.HotBarSlots[i].ItemHolder.CurrentlyHeldItem && inventoryManager.HotBarSlots[i].ItemHolder.CurrentlyHeldItem.Equals(weaponManager.EquipedWeapon))
                {
                    ActivateHotBarItem(inventoryManager.HotBarSlots[i].ItemHolder.CurrentlyHeldItem, inventoryManager.HotBarSlots[i]);

                    SelectHotBarSlot(inventoryManager.HotBarSlots[i]);

                    break;
                }

                selectedHotbarIndex = i;
                //Debug.Log(selectedHotbarIndex);

                SelectHotBarSlot(inventoryManager.HotBarSlots[i]);

                ActivateHotBarItem(inventoryManager.HotBarSlots[i].ItemHolder.CurrentlyHeldItem, inventoryManager.HotBarSlots[i]);
            }
        }
    }

    private void UpdateSelectedHotBarIndex(float direction)
    {
        if (direction == 0 /*|| !inventoryManager.HotBarSlots[selectedHotbarIndex].ItemHolder.CurrentlyHeldItem*/) return;

        if (direction > 0)
            direction = 1;

        if (direction < 0)
            direction = -1;

        for (selectedHotbarIndex -= (int)direction;
            selectedHotbarIndex < 0; selectedHotbarIndex += 6);

        while (selectedHotbarIndex >= 6)
            selectedHotbarIndex -= 6;

        SelectHotBarSlot(inventoryManager.HotBarSlots[selectedHotbarIndex]);
    }

    public void ActivateHotBarItem(Item itemToActivate, InventorySlot slot) 
    {
        if (!itemToActivate) return;

        itemPlacement.ResetPlacementMode(); //do a reset before starting a new activation

        var weaponType = itemToActivate.ItemType as Weapon;
        var consumableType = itemToActivate.ItemType as Consumable;
        var placeableType = itemToActivate.ItemType as Placeable;

        inventoryManager.UseSlot = inventoryManager.HotBarSlots[selectedHotbarIndex];
        inventoryManager.UseSlot.isActive = !inventoryManager.UseSlot.isActive;

        if (consumableType && !PlayerVitals.Instance.active)
        {
            PlayerVitals.Instance.OnEatFoodPress();
        }
        else if (weaponType)
        {
            var equipSound = weaponType.weaponSettings.EquipSound;
            weaponManager.AudioSource.PlayOneShot(equipSound, 0.5f);

            weaponManager.ToggleActiveWeapon(itemToActivate);

            EventSystem.current.SetSelectedGameObject(weaponManager.EquipedWeapon ? inventoryManager.HotBarSlots[selectedHotbarIndex].gameObject : null);
        }
        else if(placeableType)  
        {
            itemPlacement.TogglePlacementMode(itemToActivate);
        }
    }

    private void SelectHotBarSlot(InventorySlot slot)
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (slot.ItemHolder.CurrentlyHeldItem && slot.ItemHolder.CurrentlyHeldItem.ItemType is Consumable) //if food in slot, will select slot and use the food until the slot empties
        {
            if (scrollInput > 0 || scrollInput < 0) return;
            inventoryManager.SetUseSlot(slot);
            ActivateHotBarItem(slot.ItemHolder.CurrentlyHeldItem, slot);
            return;
        }

        weaponManager.ResetRig();

        if (!inventoryManager.CurrentlySelectedSlot)
        {
            if (scrollInput > 0 || scrollInput < 0)
            {
                inventoryManager.CurrentlySelectedSlot = inventoryManager.HotBarSlots[0];
                selectedHotbarIndex = 0;
            }
            else inventoryManager.CurrentlySelectedSlot = slot;

            inventoryManager.SetUseSlot(inventoryManager.CurrentlySelectedSlot);

            EventSystem.current.SetSelectedGameObject(inventoryManager.CurrentlySelectedSlot.gameObject);
        }
        else if (inventoryManager.CurrentlySelectedSlot != slot)
        {
            inventoryManager.CurrentlySelectedSlot = slot;
            inventoryManager.SetUseSlot(slot);

            EventSystem.current.SetSelectedGameObject(slot.gameObject);
        }
        else
        {
            inventoryManager.CurrentlySelectedSlot = null;
            inventoryManager.UseSlot = null;

            EventSystem.current.SetSelectedGameObject(null);
        }
    }


    public void ResetSelectedIndex(int index)
    {
        index = 0;
    }
}
