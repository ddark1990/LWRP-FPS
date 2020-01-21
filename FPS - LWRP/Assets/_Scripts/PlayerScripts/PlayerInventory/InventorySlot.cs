using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public ItemHolder ItemHolder;
    public bool isOccupied, playedOnce, isActive;

    private ItemHolder tempHolder;

    public void OnPointerEnter(PointerEventData eventData) //slot animation & sound
    {
        iTween.PunchScale(gameObject, new Vector3(.1f, .1f, .1f), .5f);
        InventoryUIController.Instance.PlayOnHoverSlotSound(.02f);
        InventoryManager.Instance.HoveringOverSlot = this;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InventoryManager.Instance.UseSlot = this;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.HoveringOverSlot = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isOccupied && ItemHolder.CurrentlyHeldItem)
        {
            InventoryManager.Instance.InfoPanelUIController.ChangeActivePanel();

            var rndNum = Random.Range(0, ItemHolder.CurrentlyHeldItem.ItemType.itemSoundData.SelectSounds.Length);
            InventoryManager.Instance.audioSource.PlayOneShot(ItemHolder.CurrentlyHeldItem.ItemType.itemSoundData.SelectSounds[rndNum], InventoryManager.Instance.uiVolume);

            InventoryManager.Instance.reset = true;
        }
    }

    public void OnDrag(PointerEventData eventData) //item drag handling
    {
        if(Input.GetKey(KeyCode.Mouse1)) //grabs a single item out of stack
        {
            //InventoryManager.Instance.GrabSingleItemFromStack(this);
            return;
        }
        else if(Input.GetKey(KeyCode.Mouse2)) //grab half of stack
        {
            //InventoryManager.Instance.GrabHalfStack(ItemHolder, this);
            return;
        }

        tempHolder = ItemHolder;

        if (!tempHolder || !tempHolder.CurrentlyHeldItem) return;

        tempHolder.canvas.sortingOrder = 7;
        tempHolder.beingDragged = true;
        tempHolder.transform.position = Input.mousePosition;

        if (InventoryManager.Instance.audioSource.isPlaying || playedOnce) return;

        InventoryManager.Instance.audioSource.PlayOneShot(tempHolder.CurrentlyHeldItem.ItemType.itemSoundData.DragSound, InventoryManager.Instance.uiVolume);
        playedOnce = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Input.GetKeyUp(KeyCode.Mouse1)) 
        {
            return;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse2)) 
        {

            return;
        }

        tempHolder.beingDragged = false;
        tempHolder.transform.localPosition = Vector3.zero;
        tempHolder.canvas.sortingOrder = 6;

        tempHolder = null;
        playedOnce = false;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (Input.GetKeyUp(KeyCode.Mouse1)) //drop a single item into a stack, onto another empty slot, or return back to stack if slot is occupied by another type
        {
            //InventoryManager.Instance.DropSingleItemOntoSlot(this);
            return;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse2)) //drop half a stack into another stack, onto another empty slot, or return back to stack if slot is occupied by another type
        {

            return;
        }

        if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, Input.mousePosition))
        {
            //logic seperated///

            if (!isOccupied)
            {
                WeaponManager.Instance.ResetRig();

                InventoryManager.Instance.MoveItem(ItemHolder, this);
            }
            else
            {
                InventoryManager.Instance.PlayerCombineItems(ItemHolder, this); //includes the swaping of items 
            }

        }
    }

}
