using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlacementSystem : MonoBehaviour
{
    public static ItemPlacementSystem Instance;

    public bool inPlacementMode;
    [Header("RayConfig")]
    public float MaxPlacementDistance = 5;
    public LayerMask groundLayer;

    [Header("Debug")]
    public bool drawDebugRay;

    private bool activeRay;
    private RaycastHit hit;
    private GameObject previewObj;


    private void OnEnable()
    {
        if (!Instance) Instance = this;
        else Destroy(this);
    }
    private void Update()
    {
        UpdatePlacementMode();
        UpdatePlacementInput();
    }

    public void TogglePlacementMode(Item _item)
    {
        inPlacementMode = !inPlacementMode;

        var placeableType = _item.ItemType as Placeable; //get the item and cast it into the type we need

        if(!previewObj) previewObj = Instantiate(placeableType.placeableSettings.PreviewObject, transform);
        else ResetPlacementMode();
    }
    public void ResetPlacementMode()
    {
        inPlacementMode = false;
        ResetPreviewObject(previewObj);
    }

    private void UpdatePlacementMode()
    {
        if (!inPlacementMode) return;

        var ray = PlayerSelection.Instance.Cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //center of screen in screen coords
        GetPlacementRaycast(ray, out hit, MaxPlacementDistance, groundLayer); //get active ray on the build layer

        //GetGroundNormal(hit.point);
        UpdatePreviewTransforms(ray);

        if (drawDebugRay) Debug.DrawRay(ray.origin, ray.direction * MaxPlacementDistance, Color.white); //debug
    }
    private void UpdatePreviewTransforms(Ray _ray)
    {
        var centerOffset = previewObj.GetComponent<BoxCollider>().center;
        var rayEndPoint = _ray.origin + _ray.direction * (MaxPlacementDistance - 2f);
        var previewPos = rayEndPoint - centerOffset;

        if (hit.collider)
        {
            previewObj.transform.position = hit.point;
            //previewObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else
        {
            previewObj.transform.position = previewPos;
            //previewObj.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, transform.forward);
        }
    }
    private void UpdatePlacementInput()
    {
        if (Input.GetKeyDown(InputManager.Instance.InputKeyManager.RotatePreviewKey)) //rotate preview object
        {
            RotatePreviewPlacement();
        }

        if(Input.GetKeyDown(InputManager.Instance.InputKeyManager.PlacementKey) && inPlacementMode)
        {
            PlacePreviewObject(InventoryManager.Instance.CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem, previewObj);
        }
    }

    private void RotatePreviewPlacement()
    {
        if (!inPlacementMode) return;

        previewObj.transform.Rotate(new Vector3(0,90,0), Space.Self);
    }
    private void PlacePreviewObject(Item itemToPlace, GameObject _previewObj) 
    {
        var objPrev = _previewObj.GetComponent<PreviewPlacementObject>();
        
        if(!objPrev.isPlaceable())
        {
            Debug.Log("Object placement obstructed.");
            return;
        }

        InventoryManager.Instance.audioSource.PlayOneShot(itemToPlace.ItemType.itemSoundData.ActivateItemSound, 0.2f);

        var _itemToPlace = itemToPlace.ItemType as Placeable;
        Instantiate(_itemToPlace.placeableSettings.ObjectToPlace, previewObj.transform.position, previewObj.transform.rotation);

        InventoryManager.Instance.RemoveItemFromInvetory(itemToPlace);

        ResetPlacementMode();
        InventoryManager.Instance.ResetCurrentlySelectedSlot();
        _itemToPlace = null;
    }

    private void ResetPreviewObject(GameObject _obj)
    {
        Destroy(previewObj);
    }

    private bool GetPlacementRaycast(Ray ray, out RaycastHit hit, float maxDistance, LayerMask layerMask)
    {
        return Physics.Raycast(ray, out hit, maxDistance, layerMask);
    }
    private void GetGroundNormal(Vector3 groundNormal) //get normal of the collider u are looking at, if nothing hit, get the up vector
    {
        groundNormal = hit.collider ? groundNormal = hit.normal : groundNormal = Vector3.up;
    }
}
