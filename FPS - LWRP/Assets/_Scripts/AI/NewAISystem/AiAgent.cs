using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace NewAISystem
{
    public class AiAgent : MonoBehaviour
    {
        public float distanceFromTarget;
        public float stoppingDistance = 0.1f;
        public float moveSpeed = 1;
        public float turnSpeed;
        public float wanderRadius = 10;
        public bool hasPath;

        private NavMeshPath _currentPath;
        private Queue<Vector3> _queuedCorners = new Queue<Vector3>();
        public Vector3 _currentDestination;
        public Vector3 _direction;
        public Vector3 velocity; 
        
        private AiSensor _aiSensor;
        private Collider _collider;
        private Rigidbody _rigidBody;

        private void Awake()
        {
            _currentDestination = transform.position;
            
            _currentPath = new NavMeshPath();
            _aiSensor = GetComponentInChildren<AiSensor>();
            _rigidBody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            GetNavMeshPath(transform.position, GetRandomRadialPos(wanderRadius), NavMesh.AllAreas);
            GetNextCorner();
        }

        private Vector3 previewPos;
        private void Update()
        {
            var position = transform.position;
            velocity = (position - previewPos) / Time.deltaTime;
            previewPos = position;
            Debug.DrawRay(transform.position + new Vector3(0,1,0), ModifySteering(_aiSensor.tempHits).normalized, Color.red);
            Debug.DrawRay(transform.position + new Vector3(0,1,0), _direction, Color.blue);

            if (_currentPath.corners.Length <= 0) return;
            
            DrawCornerLines(_currentPath.corners);
            TraverseCorner(_currentDestination);
        }
        
        private static void DrawCornerLines(IReadOnlyList<Vector3> corners)
        {
            for (int i = 0; i < corners.Count; i++)
            {
                var corner = corners[i];

                if ((i + 1) < corners.Count)
                {
                    Debug.DrawLine(corner, corners[i + 1]);
                }
            }
        }

        private void TraverseCorner(Vector3 corner)
        {
            var position = transform.position;
            var lookRotation = Quaternion.LookRotation(_direction);
            
            distanceFromTarget = (_currentDestination - position).sqrMagnitude;
            _direction = (_currentDestination - position).normalized;
            _direction.y = 0;

            
            if (distanceFromTarget > stoppingDistance && _aiSensor.tempHits.Count == 0)
            {
                //transform.Translate(_direction * (moveSpeed * Time.deltaTime), Space.World); //steer the agent towards its destination
                _rigidBody.velocity = _direction;
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed); //turn towards the direction
                //Debug.Log(_direction);
            }
            else if (_aiSensor.tempHits.Count > 0)
            {
                //transform.position = Vector3.MoveTowards(transform.position, ModifySteering(_aiSensor.tempHits), Time.deltaTime * moveSpeed);
                //_rigidBody.velocity = ModifySteering(_aiSensor.tempHits);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed); //turn towards the direction
                //Debug.Log(ModifySteering(_aiSensor.tempHits).normalized);
            }
            else GetNextCorner();
        }

        private void GetNextCorner()
        {
            if(_queuedCorners.Count > 0)
            {
                _currentDestination = _queuedCorners.Dequeue();
                _direction = transform.position - _currentDestination;
                hasPath = true;
            }
            else
            {
                _rigidBody.velocity = new Vector3(0,0,0);
                hasPath = false;
                GetNavMeshPath(transform.position, GetRandomRadialPos(wanderRadius), NavMesh.AllAreas);
            }
        }
        
        private void GetNavMeshPath(Vector3 sourcePos, Vector3 targetPos, int areaMask) //use as a way to get a path for the AI, queues up each corner 
        {
            NavMesh.CalculatePath(sourcePos, targetPos, areaMask, _currentPath);

            for (int i = 0; i < _currentPath.corners.Length; i++)
            {
                var cachedCorner = _currentPath.corners[i];

                _queuedCorners.Enqueue(cachedCorner);
            }
        }
        
        [SerializeField] private float avoidanceDistance = 0.5f;
        [SerializeField] private float maxAcceleration = 15f;

        private Vector3 ModifySteering(IEnumerable<Collider> targets)
        {
            Vector3 acceleration = Vector3.zero;

            /* 1. Find the target that the character will collide with first */

            /* The first collision time */
            float shortestTime = float.PositiveInfinity;

            /* The first target that will collide and other data that
             * we will need and can avoid recalculating */
            GameObject firstTarget = null;
            float firstMinSeparation = 0, firstDistance = 0, firstRadius = 0;
            Vector3 firstRelativePos = Vector3.zero, firstRelativeVel = Vector3.zero;

            foreach (var col in targets)
            {
                var tempAgent = col.GetComponentInParent<AiAgent>();
                
                /* Calculate the time to collision */
                var relativePos = _collider.transform.position - col.transform.position;
                var relativeVel = velocity - tempAgent.velocity;
                var distance = relativePos.magnitude;
                var relativeSpeed = relativeVel.magnitude;
                
                if (relativeSpeed == 0)
                {
                    continue;
                }

                float timeToCollision = -1 * Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

                /* Check if they will collide at all */
                Vector3 separation = relativePos + relativeVel * timeToCollision;
                float minSeparation = separation.magnitude;
                //Debug.Log(_collider.bounds.size.magnitude);

                if (minSeparation > _collider.bounds.size.magnitude + avoidanceDistance) continue;

                /* Check if its the shortest */
                if (!(timeToCollision > 0) || !(timeToCollision < shortestTime)) continue;
                
                shortestTime = timeToCollision;
                firstTarget = col.gameObject;
                firstMinSeparation = minSeparation;
                firstDistance = distance;
                firstRelativePos = relativePos;
                firstRelativeVel = relativeVel;
                firstRadius = _collider.bounds.size.magnitude;
            }

            /* 2. Calculate the steering */

            /* If we have no target then exit */
            if (firstTarget == null)
            {
                return acceleration;
            }

            /* If we are going to collide with no separation or if we are already colliding then 
             * steer based on current position */
            if (firstMinSeparation <= 0 || firstDistance < _collider.bounds.size.magnitude + firstRadius + avoidanceDistance)
            {
                acceleration = _collider.transform.position - firstTarget.transform.position;
            }
            /* Else calculate the future relative position */
            else
            {
                acceleration = firstRelativePos + firstRelativeVel * shortestTime;
            }

            /* Avoid the target */
            acceleration.y = 0;
            acceleration.Normalize();
            acceleration *= maxAcceleration;

            return acceleration;
        }
        
        private Vector3 GetRandomRadialPos(float radiusSize)
        {
            return transform.position + OnUnitSphere() * radiusSize;
        }

        private static Vector3 OnUnitSphere() //move into job
        {
            return new Vector3(UnityEngine.Random.insideUnitCircle.x, 0, UnityEngine.Random.insideUnitCircle.y);
        }
    }
}
