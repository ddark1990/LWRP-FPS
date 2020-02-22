using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlacementSystem : MonoBehaviour
{
    public static ItemPlacementSystem Instance;

    public bool inPlacementMode;
    [Header("RayConfig")]
    public float maxPlacementDistance = 5;
    public LayerMask groundLayer;

    [Header("Debug")]
    public bool drawDebugRay;

    private bool _activeRay;
    private RaycastHit _hit;
    private GameObject _previewObj;

    private InputManager _inputManager;
    private InventoryManager _inventoryManager;
    private PlayerSelection _playerSelection;

    private void Start()
    {
        _inputManager = InputManager.Instance;
        _inventoryManager = InventoryManager.Instance;
    }

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

    public void TogglePlacementMode(Item item)
    {
        inPlacementMode = !inPlacementMode;

        var placeableType = item.ItemType as Placeable; //get the item and cast it into the type we need

        if(!_previewObj) _previewObj = Instantiate(placeableType.placeableSettings.PreviewObject, transform);
        else ResetPlacementMode();
    }
    public void ResetPlacementMode()
    {
        inPlacementMode = false;
        ResetPreviewObject(_previewObj);
    }

    private void UpdatePlacementMode()
    {
        if (!inPlacementMode) return;
        
        var ray = PlayerSelection.Instance.cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //center of screen in screen coords
        GetPlacementRaycast(ray, out _hit, maxPlacementDistance, groundLayer); //get active ray on the build layer

        //GetGroundNormal(_hit.point);
        UpdatePreviewTransforms(ray);

        if (drawDebugRay) Debug.DrawRay(ray.origin, ray.direction * maxPlacementDistance, Color.white); //debug
    }
    private void UpdatePreviewTransforms(Ray ray)
    {
        var centerOffset = _previewObj.GetComponent<BoxCollider>().center; //every placeable must be placed with a box collider
        var rayEndPoint = ray.origin + ray.direction * (maxPlacementDistance - 2f);
        var previewPos = rayEndPoint - centerOffset;

        if (_hit.collider)
        {
            _previewObj.transform.position = _hit.point;
            //_previewObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, _hit.normal);
        }
        else
        {
            _previewObj.transform.position = previewPos;
            //previewObj.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, transform.forward);
        }
    }
    
    private void UpdatePlacementInput()
    {
        if (Input.GetKeyDown(_inputManager.InputKeyManager.RotatePreviewKey)) //rotate preview object
        {
            RotatePreviewPlacement();
        }
        if(Input.GetKeyDown(_inputManager.InputKeyManager.PlacementKey) && inPlacementMode)
        {
            PlacePreviewObject(_inventoryManager.CurrentlySelectedSlot.ItemHolder.CurrentlyHeldItem, _previewObj);
        }
    }

    private void RotatePreviewPlacement()
    {
        if (!inPlacementMode) return;

        _previewObj.transform.Rotate(new Vector3(0,90,0));
    }
    private void PlacePreviewObject(Item itemToPlace, GameObject _previewObj) 
    {
        var objPrev = _previewObj.GetComponent<PreviewPlacementObject>();
        
        if(!objPrev.IsPlaceable())
        {
            Debug.Log("Object placement obstructed.");
            return;
        }

        _inventoryManager.audioSource.PlayOneShot(itemToPlace.ItemType.itemSoundData.ActivateItemSound, 0.2f);

        var _itemToPlace = itemToPlace.ItemType as Placeable;
        Instantiate(_itemToPlace.placeableSettings.ObjectToPlace, this._previewObj.transform.position, this._previewObj.transform.rotation);

        _inventoryManager.RemoveItemFromInventory(itemToPlace);

        ResetPlacementMode();
        _inventoryManager.ResetCurrentlySelectedSlot();
        _itemToPlace = null;
    }

    private void ResetPreviewObject(GameObject _obj)
    {
        Destroy(_previewObj);
    }

    private bool GetPlacementRaycast(Ray ray, out RaycastHit hit, float maxDistance, LayerMask layerMask)
    {
        return Physics.Raycast(ray, out hit, maxDistance, layerMask);
    }
    private void GetGroundNormal(Vector3 groundNormal) //get normal of the collider u are looking at, if nothing hit, get the up vector
    {
        groundNormal = _hit.collider ? groundNormal = _hit.normal : groundNormal = Vector3.up;
    }
}
