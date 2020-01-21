using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    public RectTransform invPanel;

    public void OnDrop(PointerEventData eventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition))
        {
            //Debug.Log("Item Dropped");

            InventoryManager.Instance.DropItem();

            WeaponManager.Instance.ResetRig();
        }
    }
}
