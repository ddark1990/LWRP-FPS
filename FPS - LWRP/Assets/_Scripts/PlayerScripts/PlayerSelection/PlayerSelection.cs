using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class PlayerSelection : MonoBehaviour
{
    public static PlayerSelection Instance;

    public GameObject objectInView;
    public GameObject interactableInView;
    public Item itemInView;

    [SerializeField] private float sphereCheckRadius = 0.1f;
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private LayerMask layerMask;

    [Header("Cache")]
    public bool drawDebugRay;

    [HideInInspector] public Camera cam;
    private RaycastHit _hit;
    private Vector3 _origin;
    private Vector3 _direction;
    private Transform _transform;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        cam = GetComponent<Camera>();
        _transform = transform;
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetObjectInView), .1f, .1f);
    }


    private float _currentHitDistance;
    private void GetObjectInView()
    {
        if (PhotonNetwork.IsConnected && !transform.root.GetComponent<PhotonView>().IsMine) return;
        
        itemInView = null;
        objectInView = null;
        interactableInView = null;
        
        _origin = _transform.position;
        _direction = _transform.forward;

        if (!Physics.SphereCast(_origin, sphereCheckRadius, _direction, out _hit, maxDistance,
            layerMask))
        {
            _currentHitDistance = maxDistance;
            return;
        }

        _currentHitDistance = _hit.distance;

        itemInView = _hit.collider.GetComponentInParent<Item>();
        objectInView = _hit.rigidbody.gameObject;
        
        GameObject go;
        interactableInView = (go = _hit.rigidbody.gameObject).layer.Equals(LayerMask.NameToLayer("Interactable")) ? go : null;

    }

    public void InteractWithObject()
    {
        if (!interactableInView && !Input.GetKeyDown(KeyCode.E)) return;

       
    }

    private void ApplyOutline()
    {
        if (!itemInView) return;

        var objRend = itemInView.GetComponentInChildren<MeshRenderer>();
        var oldShader = objRend.material.shader;
    } //fix

    private void OnDrawGizmos()
    {
        if (!drawDebugRay) return;
        
        Gizmos.DrawLine(_origin, _origin + _direction * _currentHitDistance);
        Gizmos.DrawWireSphere(_origin + _direction * _currentHitDistance, sphereCheckRadius);
        
    }
}
