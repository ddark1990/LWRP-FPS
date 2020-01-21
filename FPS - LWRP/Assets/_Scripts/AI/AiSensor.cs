using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SphereCollider))]
public class AiSensor : MonoBehaviour
{
    public float sensorRadius;

    [HideInInspector] public SphereCollider sensorCollider;
    public List<Collider> tempHits;

    private void Start()
    {
        sensorCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        sensorCollider.radius = sensorRadius;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other == null || !other.gameObject.layer.Equals(LayerMask.NameToLayer("NPC"))) return;

        tempHits.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null || !other.gameObject.layer.Equals(LayerMask.NameToLayer("NPC"))) return;
        
        tempHits.Remove(other);
    }
}
