using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class AiTargetManager : MonoBehaviour
{
    [Range(0, 100)] public float RadiusSize;
    public float ClosestTargetDistance;
    public List<ObjectInRange> objectsInRange;

    [Header("Cache")]
    public SphereCollider sphereCollider;


    private void OnEnable()
    {
        Init();
    }

    private void Update()
    {
        sphereCollider.radius = RadiusSize;
    }

    private void OnTriggerEnter(Collider collider)
    {
        var objInRange = new ObjectInRange();
        objInRange.gameObject = collider.gameObject;
        objInRange.Name = collider.gameObject.name;

        objectsInRange.Add(objInRange);
    }

    private void OnTriggerStay(Collider collider)
    {
        for (int i = 0; i < objectsInRange.Count; i++)
        {
            var obj = objectsInRange[i];

            ClosestTargetDistance = Vector3.Distance(transform.position, obj.gameObject.transform.position);
            //obj.DistanceAway = (transform.position - obj.gameObject.transform.position).magnitude;

            if (obj.gameObject.Equals(collider.gameObject))
            {
                //var g = GetClosestObject(objectsInRange, transform.position);
                //Debug.DrawLine(transform.position, g.transform.position, Color.red);
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        for (int i = 0; i < objectsInRange.Count; i++)
        {
            var obj = objectsInRange[i];

            if(collider.gameObject.Equals(obj.gameObject))
            {
                objectsInRange.Remove(obj);
                
            }
        }
    }

    private float GetDistanceBetween(float x, float y)
    {
        return x- y;
    }
    private float GetDistanceBetween(Vector3 p, Vector3 u)
    {
        return Vector3.Distance(p, u);
    }
    private GameObject GetClosestObject(List<ObjectInRange> objsInRange, Vector3 position)
    {
        GameObject closestObj = null;
        var distBetween = 0f;
        var minDist = Mathf.Infinity;

        for (int i = 0; i < objsInRange.Count; i++)
        {
            var obj = objsInRange[i];
            distBetween = Vector3.Distance(position, obj.gameObject.transform.position);

            if(distBetween < minDist)
            {
                closestObj = obj.gameObject;
                minDist = distBetween;
            }
        }

        return closestObj;
    }

    private void Init()
    {
        sphereCollider.isTrigger = true;
    }

    [Serializable]
    public struct ObjectInRange
    {
        public string Name;
        public GameObject gameObject;
    }

}
