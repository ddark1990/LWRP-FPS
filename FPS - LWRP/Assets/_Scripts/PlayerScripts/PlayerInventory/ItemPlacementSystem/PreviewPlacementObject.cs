using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPlacementObject : MonoBehaviour
{
    private List<Collider> collidersHit = new List<Collider>();

    [Header("Cache")]
    [SerializeField] private Renderer rend;
    [SerializeField] private Material placeableMaterial;
    [SerializeField] private Material nonPlaceableMaterial;

    private void Update()
    {
        UpdatePlaceableColor(rend);
        isGrounded();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != 9)
        {
            collidersHit.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 9)
        {
            collidersHit.Remove(other);
        }
    }

    public bool isPlaceable()
    {
        return collidersHit.Count == 0 && isGrounded() ? true : false;
    }
    public bool isGrounded()
    {
        var startingRayPos = 0.01f;
        var rayDistance = 0.02f;

        var rayOrig = transform.TransformPoint(new Vector3(0, startingRayPos, 0));

        Physics.Raycast(rayOrig, new Vector3(0, -1, 0), out var hit, rayDistance, ItemPlacementSystem.Instance.groundLayer);

        //if (a) Debug.DrawRay(z, new Vector3(0, rayDistance, 0), Color.green); //debug
        //else Debug.DrawRay(z, new Vector3(0, rayDistance, 0), Color.red);

        return hit.collider ? true : false;
    }
    private void UpdatePlaceableColor(Renderer _rend)
    {
        if(isPlaceable())
        {
            _rend.material = placeableMaterial;
        }
        else
        {
            _rend.material = nonPlaceableMaterial;
        }
    }
}
