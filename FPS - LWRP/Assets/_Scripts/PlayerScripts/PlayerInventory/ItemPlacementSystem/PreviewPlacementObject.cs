using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPlacementObject : MonoBehaviour
{
    private List<Collider> _collidersHit = new List<Collider>();

    [Header("Cache")]
    [SerializeField] private Renderer rend;
    [SerializeField] private Material placeableMaterial;
    [SerializeField] private Material nonPlaceableMaterial;

    private void Update()
    {
        UpdatePlaceableColor(rend);
        IsGrounded();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != 9)
        {
            _collidersHit.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 9)
        {
            _collidersHit.Remove(other);
        }
    }

    public bool IsPlaceable()
    {
        return _collidersHit.Count == 0 && IsGrounded() ? true : false;
    }

    private bool IsGrounded()
    {
        var startingRayPos = 0.01f;
        var rayDistance = 0.02f;

        var rayOrig = transform.TransformPoint(new Vector3(0, startingRayPos, 0));

        Physics.Raycast(rayOrig, new Vector3(0, -1, 0), out var hit, rayDistance, ItemPlacementSystem.Instance.groundLayer);

        //if (a) Debug.DrawRay(z, new Vector3(0, rayDistance, 0), Color.green); //debug
        //else Debug.DrawRay(z, new Vector3(0, rayDistance, 0), Color.red);

        return hit.collider ? true : false;
    }
    private void UpdatePlaceableColor(Renderer rend)
    {
        rend.material = IsPlaceable() ? placeableMaterial : nonPlaceableMaterial;
    }
}
