using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventorySlot CurrentlySelectedSlot;
    public InventorySlot HoveringOverSlot;
    [Header("Items Inside Inventory")]
    public List<Item> HoldingItems;

    [Header("Cache")]
    public InventorySlot[] InventorySlots;
    public InventorySlot[] HotBarSlots;
    public AudioSource audioSource;
    public ItemHolder TempHolder;
    public InfoPanelUIController InfoPanelUIController;
    public HotbarController HotbarController;

    [Header("etc")]
    public float uiVolume; //move
    public ItemHolder deductedHolder;
    public InventorySlot UseSlot;
    [HideInInspector] public bool reset = false;

    public bool MenuOpen;

    #region Unity Methods
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        audioSource = GetComponent<AudioSource>();

        InvokeRepeating("GetCurrentlySelectSlot", 0.1f, 0.1f);
    }
    private void Update()
    {
        PickUpItem(PlayerSelection.Instance.itemInView);
    }
    #endregion


    public void UpdateItemInsideInventory(Item itemToUpdate, InventorySlot[] invSlots, InventorySlot[] hotbarSlots) //check both slot arrays where the item is and update its data
    {
        UpdateItemData(itemToUpdate, invSlots);
        UpdateItemData(itemToUpdate, hotbarSlots);
        //Debug.Log("Updating Item Inside Inventory");
    }
    private void UpdateItemData(Item _item, InventorySlot[] slotList) //update items data once used 
    {
        for (int i = 0; i < slotList.Length; i++)
        {
            var slot = slotList[i];

            if (slot.isOccupied && slot.ItemHolder.CurrentlyHeldItem.Equals(_item)) //checks occupied slots if the item being picked up is the same as the holding item and sets its item count based on a new one
            {
                _item.ItemAmount = slot.ItemHolder.ItemAmount;
            }
        }
    }

    public void RemoveItemFromInvetory(Item itemToRemove)
    {
        HoldingItems.Remove(itemToRemove); 
        Destroy(itemToRemove.gameObject); //removes the items gameobject from the playersprefab
    }

    public void AddItemToSlot(Item itemInView) //updates items that are visible to the player inside the inventory
    {
        if (itemInView.ItemType is Weapon || itemInView.ItemType is Consumable)
        {
            AddToSlotLogic(itemInView, HotBarSlots);
        }
        else
        {
            AddToSlotLogic(itemInView, InventorySlots);
        }

        UpdateItemInsideInventory(itemInView, InventorySlots, HotBarSlots);
    }
    private void AddToSlotLogic(Item itemInView, InventorySlot[] slotArr)
    {
        for (int i = 0; i < slotArr.Length; i++)
        {
            var slot = slotArr[i];
            var itemHolder = slot.GetComponentInChildren<ItemHolder>();

            if (inventoryContainsType(slotArr, itemInView) && isStackableType(itemInView))
            {
                MoveLeftOverStack(slotArr, itemInView, itemHolder);
                break;
            }
            else if (!slot.isOccupied && !itemHolder.CurrentlyHeldItem)
            {
                AddItemToInventory(itemInView, itemHolder, slot);
                break;
            }
        }
    }
    private bool inventoryContainsType(InventorySlot[] slotArr, Item _item)
    {
        for (int i = 0; i < slotArr.Length; i++)
        {
            var slot = slotArr[i];

            if(slot.isOccupied && slot.ItemHolder.CurrentlyHeldItem.ItemType.Equals(_item.ItemType))
            {
                return true;
            }
        }

        return false;
    }
    private void MoveLeftOverStack(InventorySlot[] slotArr, Item itemInView, ItemHolder itemHolder) // need to combine into other stacks too
    {
        var combinedAmount = 0;
        var stackLimit = 0;
        var leftOver = 0;
        var stackAmount = 0;

        for (int i = 0; i < slotArr.Length; i++)
        {
            var slot = slotArr[i];

            if (slot.isOccupied && slot.ItemHolder.CurrentlyHeldItem.ItemType.Equals(itemInView.ItemType) && isNotFullStack(slot.ItemHolder))
            {
                if (canFitStack(itemHolder, itemInView))
                {
                    CombineItems(itemHolder, itemInView); 
                    return;
                }

                combinedAmount = slot.ItemHolder.ItemAmount + itemInView.ItemAmount; //get combined amount of both items to get the left over
                stackLimit = itemInView.ItemType.itemData.StackLimit;

                if(combinedAmount > stackLimit)
                    leftOver = combinedAmount - itemInView.ItemType.itemData.StackLimit; //get left over 

                stackAmount = combinedAmount - leftOver; //should equal out to the items max stack size

                slot.ItemHolder.ItemAmount = stackAmount;
                slot.ItemHolder.CurrentlyHeldItem.ItemAmount = stackAmount;

                var nextOpenSlot = GetUnoccupiedSlot(slotArr);
                var nextOpenSlotItemHolder = nextOpenSlot.GetComponentInChildren<ItemHolder>();

                if(leftOver > 0)
                {
                    nextOpenSlotItemHolder.CurrentlyHeldItem = itemInView;
                    nextOpenSlotItemHolder.ItemAmount = leftOver;
                    nextOpenSlot.isOccupied = true;
                }
                
                break;
            }

        }
    }

    private void AddItemToInventory(Item itemInView, ItemHolder itemHolder, InventorySlot invSlot)
    {
        itemHolder.CurrentlyHeldItem = itemInView; //additemtoinventory
        itemHolder.ItemAmount += itemInView.ItemAmount;

        invSlot.isOccupied = true;
        //Debug.Log("Adding " + itemInView);
    }

    private void StackIntoNextSlot(InventorySlot[] slotArr, Item itemInView, ItemHolder itemHolder, int leftOver)
    {
        var stackAmmount = (itemInView.ItemAmount + itemHolder.ItemAmount) - leftOver; //stackintonextslot

        itemHolder.ItemAmount = stackAmmount;

        UpdateItemInsideInventory(itemHolder.CurrentlyHeldItem, InventorySlots, HotBarSlots);

        var unoccupiedSlot = GetUnoccupiedSlot(slotArr);
        var unoccupiedSlotItemHolder = unoccupiedSlot.GetComponentInChildren<ItemHolder>();

        unoccupiedSlotItemHolder.CurrentlyHeldItem = itemInView;
        unoccupiedSlotItemHolder.ItemAmount = leftOver;

        unoccupiedSlot.isOccupied = true;

        //Debug.Log("Stacking into next slot.");
    }


    public void PickUpItem(Item itemInView)
    {
        if (!itemInView) return;

        if (Input.GetKeyDown(InputManager.Instance.InputKeyManager.InteractableKey))
        {
            if (HoldingItems.Contains(itemInView)) return; //prevent getting the same item multiple times from spamming the pick up key

            if (itemInView.GetComponent<IInteractable>() != null) //interface init
                itemInView.GetComponent<IInteractable>().OnPickedUp();

            audioSource.PlayOneShot(itemInView.ItemType.itemSoundData.PickUpItemSound, uiVolume); //play pick up item sound

            //PlayerSelection.Instance.SelectableObjects.Remove(itemInView.gameObject);

            AddItemToSlot(itemInView);
            HoldingItems.Add(itemInView); //add iteminview to inventory data list

            itemInView.gameObject.SetActive(false); //world object turn off
            itemInView.transform.SetParent(transform); //item transform parented to the players inventory manager gameobject
            itemInView.transform.position = transform.position; //items transform reset to players 0,0,0 local pos & rot
            itemInView.transform.rotation = Quaternion.identity;

        }
    }
    public void DropItem()
    {
        for (int i = 0; i < HoldingItems.Count; i++)
        {
            var item = HoldingItems[i];

            if(item.Equals(UseSlot.ItemHolder.CurrentlyHeldItem))
            {
                var cameraForward = transform.root.GetComponentInChildren<Camera>().transform.forward;

                item.transform.position = transform.root.GetComponentInChildren<Camera>().ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 1));
                item.gameObject.SetActive(true);
                item.GetComponent<Rigidbody>().AddForce(transform.parent.forward * 2, ForceMode.VelocityChange);
                item.transform.parent = null;

                HoldingItems.Remove(item);

                audioSource.PlayOneShot(UseSlot.ItemHolder.CurrentlyHeldItem.ItemType.itemSoundData.ThrowItemSound, uiVolume); //move to audio manager
                break;
            }
        }

        ResetUseSlot();
    }
    public void MoveItem(ItemHolder _itemHolder, InventorySlot invSlot)
    {
        if (!CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem) return;

        _itemHolder.ItemAmount = CurrentlySelectedSlot.ItemHolder.ItemAmount;
        _itemHolder.CurrentlyHeldItem = CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem;

        if (_itemHolder.CurrentlyHeldItem)
            audioSource.PlayOneShot(_itemHolder.CurrentlyHeldItem.ItemType.itemSoundData.DropSound, uiVolume); //play sound

        CurrentlySelectedSlot.ItemHolder.ItemAmount = 0;
        CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem = null;
        CurrentlySelectedSlot.isOccupied = false;

        invSlot.isOccupied = true;
    }
    public void PlayerCombineItems(ItemHolder _itemHolder, InventorySlot onDropSlot) //player controlled combine
    {
        if (!sameItemType(_itemHolder, CurrentlySelectedSlot.ItemHolder)) //check if same type of item & not already a full stack
        {
            SwapItems(_itemHolder, onDropSlot);

            return;
        }

        var tilFullStack = 0;

        if (fitsIntoStack(_itemHolder, CurrentlySelectedSlot.ItemHolder, out tilFullStack))
        {
            _itemHolder.ItemAmount += CurrentlySelectedSlot.ItemHolder.ItemAmount;
            UpdateItemInsideInventory(_itemHolder.CurrentlyHeldItem, InventorySlots, HotBarSlots);

            CurrentlySelectedSlot.ItemHolder.ItemAmount = 0;
            CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem = null;

            CurrentlySelectedSlot.isOccupied = false;
        }
        else
        {
            if (CurrentlySelectedSlot.ItemHolder.ItemAmount >= _itemHolder.ItemAmount || CurrentlySelectedSlot.ItemHolder.ItemAmount <= _itemHolder.ItemAmount)
            {
                var leftOver = _itemHolder.CurrentlyHeldItem.ItemType.itemData.StackLimit - _itemHolder.ItemAmount;

                _itemHolder.ItemAmount += leftOver;
                CurrentlySelectedSlot.ItemHolder.ItemAmount -= leftOver;
                UpdateItemInsideInventory(CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem, InventorySlots, HotBarSlots);
            }
        }
        UpdateItemInsideInventory(_itemHolder.CurrentlyHeldItem, InventorySlots, HotBarSlots);

        //play sound
    }
    private void SwapItems(ItemHolder _itemHolder, InventorySlot onDropSlot)
    {
        var tempItem = _itemHolder.CurrentlyHeldItem;
        var tempAmount = _itemHolder.ItemAmount;

        _itemHolder.CurrentlyHeldItem = CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem;
        _itemHolder.ItemAmount = CurrentlySelectedSlot.ItemHolder.ItemAmount;

        CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem = tempItem;
        CurrentlySelectedSlot.ItemHolder.ItemAmount = tempAmount;

        audioSource.PlayOneShot(_itemHolder.CurrentlyHeldItem.ItemType.itemSoundData.DropSound, 0.1f); //play sound

        tempItem = null;
        tempAmount = 0;
    }
    private void CombineItems(ItemHolder _itemHolder, Item itemToCombine) //auto combine when picking up items
    {
        _itemHolder.ItemAmount += itemToCombine.ItemAmount;
        _itemHolder.CurrentlyHeldItem.ItemAmount = _itemHolder.ItemAmount;

        HoldingItems.Remove(itemToCombine);
        Destroy(itemToCombine.gameObject);

        //Debug.Log("Stacking " + _itemHolder.CurrentlyHeldItem);
    }

    public void SetUseSlot(InventorySlot invSlot)
    {
        UseSlot = invSlot;
    }

    public void ResetUseSlot()
    {
        UseSlot.ItemHolder.CurrentlyHeldItem = null;
        UseSlot.ItemHolder.ItemAmount = 0;
        UseSlot.isOccupied = false;
        UseSlot = null;
        InfoPanelUIController.ResetAllPanels();
    }
    public void ResetCurrentlySelectedSlot()
    {
        CurrentlySelectedSlot.isActive = false;
        CurrentlySelectedSlot.isOccupied = false;
        CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem = null;

        EventSystem.current.SetSelectedGameObject(null);
        CurrentlySelectedSlot = null;
        ResetUseSlot();
    }
    public float GetUIVolumeNormalized(int value) //move
    {
        var uiVolMax = 0.5f;
        return uiVolume = (value * uiVolMax) / 100;
    }

    #region Helper Functions
    private InventorySlot GetCurrentlySelectSlot()
    {
        if (!EventSystem.current.currentSelectedGameObject && reset || CurrentlySelectedSlot && !CurrentlySelectedSlot.isOccupied && reset)
        {
            InfoPanelUIController.ResetAllPanels();
            CurrentlySelectedSlot = null;
            reset = false;
            return null;
        }
        else if (EventSystem.current.currentSelectedGameObject)
        {
            CurrentlySelectedSlot = EventSystem.current.currentSelectedGameObject.GetComponent<InventorySlot>();
            return CurrentlySelectedSlot;
        }

        return null;
    }
    private InventorySlot GetUnoccupiedSlot(InventorySlot[] slotArr)
    {
        for (int i = 0; i < slotArr.Length; i++)
        {
            var slot = slotArr[i];

            if (!slot.isOccupied) return slot;
        }

        return null;
    }
    private bool isStackableType (Item item)
    {
        return item.ItemType.itemData.StackLimit > 1;
    }
    private bool isNotFullStack(ItemHolder itemHolder)
    {
        return !itemHolder.ItemAmount.Equals(itemHolder.CurrentlyHeldItem.ItemType.itemData.StackLimit);
    }
    private bool canFitStack(ItemHolder itemHolder, Item item)
    {
        return itemHolder.CurrentlyHeldItem && itemHolder.ItemAmount + item.ItemAmount <= itemHolder.CurrentlyHeldItem.ItemType.itemData.StackLimit;
    }
    private bool stackHasLeftOver(ItemHolder itemHolder, Item itemInView, out int leftOver)
    {
        leftOver = itemInView.ItemAmount + itemHolder.ItemAmount - itemInView.ItemType.itemData.StackLimit;

        return itemInView.ItemAmount + itemHolder.ItemAmount > itemInView.ItemType.itemData.StackLimit;
    }
    private bool fitsIntoStack(ItemHolder itemHolder, ItemHolder dropingHolder, out int amountTilFull)
    {
        amountTilFull = itemHolder.CurrentlyHeldItem.ItemType.itemData.StackLimit - (itemHolder.ItemAmount + dropingHolder.ItemAmount);

        return itemHolder.ItemAmount + dropingHolder.ItemAmount <= itemHolder.CurrentlyHeldItem.ItemType.itemData.StackLimit;
    }
    private bool sameItemType(ItemHolder itemHolder, ItemHolder dropingHolder)
    {
        return itemHolder.CurrentlyHeldItem.ItemType == dropingHolder.CurrentlyHeldItem.ItemType;
    }
    #endregion

    #region FINISHLATER
    public void GrabSingleItemFromStack(InventorySlot onDropSlot) //right click
    {
        TempHolder.transform.position = Input.mousePosition;

        if (TempHolder.CurrentlyHeldItem || !HoveringOverSlot) return;

        if (HoveringOverSlot.ItemHolder.ItemAmount >= 1)
        {
            HoveringOverSlot.ItemHolder.ItemAmount -= 1;

            TempHolder.CurrentlyHeldItem = HoveringOverSlot.ItemHolder.CurrentlyHeldItem;
            TempHolder.ItemAmount = 1;

            deductedHolder = HoveringOverSlot.ItemHolder;

            if (HoveringOverSlot.ItemHolder.ItemAmount == 0)
            {
                HoveringOverSlot.ItemHolder.CurrentlyHeldItem = null;
                HoveringOverSlot.isOccupied = false;
            }
        }
    }
    public void DropSingleItemOntoSlot(InventorySlot onDropSlot)
    {
        if (!onDropSlot.isOccupied)
        {
            onDropSlot.ItemHolder.CurrentlyHeldItem = TempHolder.CurrentlyHeldItem;
            onDropSlot.ItemHolder.ItemAmount += TempHolder.ItemAmount;
            onDropSlot.isOccupied = true;

            ResetItemHolder(TempHolder);
            deductedHolder = null;
        }
        else if (onDropSlot.isOccupied && sameItemType(onDropSlot.ItemHolder, TempHolder) && onDropSlot.ItemHolder.ItemAmount != onDropSlot.ItemHolder.CurrentlyHeldItem.ItemType.itemData.StackLimit)
        {
            onDropSlot.ItemHolder.ItemAmount += 1;

            ResetItemHolder(TempHolder);
        }
        else if (onDropSlot.isOccupied && !sameItemType(onDropSlot.ItemHolder, TempHolder)) //fix 
        {
            deductedHolder.ItemAmount += 1;

            ResetItemHolder(TempHolder);
        }
    }
    private void ResetItemHolder(ItemHolder itemHolder)
    {
        itemHolder.ItemAmount = 0;
        itemHolder.CurrentlyHeldItem = null;
    }

    public void GrabHalfStack(ItemHolder _itemHolder, InventorySlot onDropSlot) //middle click
    {

    }
    #endregion

}
