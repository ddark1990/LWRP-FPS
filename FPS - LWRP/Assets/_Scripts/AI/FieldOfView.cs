using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("Focus")]
    //public Transform focus;
    public Collider[] resultTargetArr;
    [Header("Radius Settings")]
    [Range(0, 360)] public float viewRadius;
    [Range(0, 360)] public float viewAngle;
    [SerializeField] private LayerMask targetMask;

    [HideInInspector] public float meshRes;
    [HideInInspector] public int edgeResolveIteratons;
    [HideInInspector] public float edgeDistThreshold;
    public float rayHeight = 1f;

    private readonly Mesh _viewMesh;
    private AiStateController _aiStateController;
    //private Collider[] targetsInViewRadius;
    private bool _foundTarget;

    //public bool itemInView;
    //public Transform firstSeenItem;
    
    public FieldOfView(Mesh viewMesh)
    {
        this._viewMesh = viewMesh;
    }

    private void OnEnable()
    {
        EventRelay.OnSeenTarget += targetInViewEvent;
        EventRelay.LostVisionOfTarget += lostTargetInViewEvent;
    }
    private void OnDisable()
    {
        EventRelay.OnSeenTarget -= targetInViewEvent;
        EventRelay.LostVisionOfTarget -= lostTargetInViewEvent;
    }

    private void Start()
    {
        resultTargetArr = new Collider[50];
        _aiStateController = GetComponent<AiStateController>();
    }

    private void Update()
    {
        if (_aiStateController.aiVitals.isDead) return;

        FindVisibleTargets();
    }

    public void InitializeFov(float _viewRaius, float _viewAngle)
    {
        viewRadius = _viewRaius;
        viewAngle = _viewAngle;
    }

    private int _lastIndex;
    private int _numOfHits;
    
    private void FindVisibleTargets()
    {
        //firstSeenItem = null;
        //itemInView = false;
        
        Array.Clear(resultTargetArr, 0, resultTargetArr.Length);
            
        _numOfHits = Physics.OverlapSphereNonAlloc(transform.position, viewRadius, resultTargetArr, targetMask);
        
        for (var i = 0; i < _numOfHits; i++)
        {
            var target = resultTargetArr[i].transform;
            
            var dirToTarget = (target.position - transform.position).normalized;

            if (!(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)) continue;
            
            //var distToTarget = Vector3.Distance(transform.position, target.position);
                
            if (Physics.Raycast(transform.position + new Vector3(0, rayHeight, 0),
                dirToTarget + new Vector3(0, rayHeight, 0), _aiStateController.distanceFromTarget, targetMask)) continue; //change target mask when obstacles are present

            // if (!itemInView && target && hasItem)
            // {
            //     itemInView = true;
            //     firstSeenItem = target;
            //     Debug.Log("itemInView");
            //    // _aiStateController.target = firstSeenItem;
            // }
            
            // if(target != transform)
            //     visibleTargets.Add(target);

            if(!_foundTarget && target != transform) //if we see a target
            {
                _aiStateController.target = target;
                _foundTarget = true;


                targetInViewEvent(target); //found target
                //Debug.Log(target.name + " In View", this);
            }

            //Debug.DrawLine(transform.position + new Vector3(0, testfloat, 0), focus.position + new Vector3(0, testfloat, 0), Color.green);
        }
        
    }

    public void targetInViewEvent(Transform target)
    {
        Debug.Log("Seen " + target.name);
    }

    public void lostTargetInViewEvent(Transform target)
    {
        Debug.Log("Lost track of " + target.name);
    }
    
    private void DrawFieldOfView() //draws the mesh to display the angle and radius
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshRes);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDistThresholdExceeded = Mathf.Abs(oldViewCast.dis - newViewCast.dis) > edgeDistThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }
    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngel = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIteratons; i++)
        {
            float angle = (minAngle + maxAngel) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistThresholdExceeded = Mathf.Abs(minViewCast.dis - newViewCast.dis) > edgeDistThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngel = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }
    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, targetMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dis;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dis, float _angle)
        {
            hit = _hit;
            point = _point;
            dis = _dis;
            angle = _angle;
        }
    }
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(transform.position, viewRadius);
        Gizmos.DrawRay(transform.position + new Vector3(0,rayHeight,0), transform.forward);
    }
}
