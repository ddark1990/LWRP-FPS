using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class AiAvoidance : MonoBehaviour
{
    [SerializeField] private float avoidanceDistance = 0.5f;
    [SerializeField] private float maxAcceleration = 15f;

    private NavMeshAgent _agent;
    private SphereCollider _collider;
    private ThirdPersonCharacter _thirdPersonCharacter;
    private AiSensor _aiSensor;
    
    private void Awake()
    {
        GetComponents();
    }

    private void GetComponents()
    {
        _agent = GetComponent<NavMeshAgent>();
        _thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        _aiSensor = GetComponentInChildren<AiSensor>();
        _collider = GetComponentInChildren<SphereCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (_aiSensor.tempHits.Count > 0 && _agent.hasPath)
        {
            //_thirdPersonCharacter.SetRoot = false;
            //_agent.isStopped = true;
            //_thirdPersonCharacter.Move(_agent.velocity + GetSteering(_aiSensor.tempHits), false, false);
            //Debug.DrawRay(transform.position + new Vector3(0,2,0), GetSteering(_aiSensor.tempHits), Color.red);
            //Debug.Log(GetSteering());
        }
        else
        {
            //_agent.isStopped = false;
            //_thirdPersonCharacter.SetRoot = true;
        }
    }
    
    public Vector3 ModifySteering(IEnumerable<SphereCollider> targets)
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

            foreach (var r in targets)
            {
                var tempAgent = r.GetComponentInParent<NavMeshAgent>();
                /* Calculate the time to collision */
                Vector3 relativePos = _collider.transform.position - r.transform.position;
                Vector3 relativeVel = _agent.velocity - tempAgent.velocity;
                float distance = relativePos.magnitude;
                float relativeSpeed = relativeVel.magnitude;
                
                //Debug.Log(relativeVel);
                
                if (relativeSpeed == 0)
                {
                    continue;
                }

                float timeToCollision = -1 * Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

                /* Check if they will collide at all */
                Vector3 separation = relativePos + relativeVel * timeToCollision;
                float minSeparation = separation.magnitude;

                if (minSeparation > _collider.radius + r.radius + avoidanceDistance)
                {
                    continue;
                }

                /* Check if its the shortest */
                if (timeToCollision > 0 && timeToCollision < shortestTime)
                {
                    shortestTime = timeToCollision;
                    firstTarget = r.gameObject;
                    firstMinSeparation = minSeparation;
                    firstDistance = distance;
                    firstRelativePos = relativePos;
                    firstRelativeVel = relativeVel;
                    firstRadius = r.radius;
                }
            }

            /* 2. Calculate the steering */

            /* If we have no target then exit */
            if (firstTarget == null)
            {
                return acceleration;
            }

            /* If we are going to collide with no separation or if we are already colliding then 
             * steer based on current position */
            if (firstMinSeparation <= 0 || firstDistance < _collider.radius + firstRadius + avoidanceDistance)
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
    
    public NavMeshPath pathToUse;
    public Queue<Vector3> cornerQueue;
    public Vector3 currentDestination;
    bool hasPath;
    public float currentDistance;
    public float minDistanceArrived;
    Vector3 direction;
    public float moveSpeed;

    // get the corners and add them to a queue for use to use
    public void SetupPath(NavMeshPath path)
    {
        cornerQueue = new Queue<Vector3>();
        foreach(Vector3 corner in path.corners)
        {
            cornerQueue.Enqueue(corner);
            Debug.Log(corner);
        }

        GetNextCorner();
        currentDistance = (transform.position - currentDestination).sqrMagnitude;
        hasPath = true;
    }
 
    // get the next corner, and set direction
    void GetNextCorner()
    {
        if(cornerQueue.Count > 0)
        {
            currentDestination = cornerQueue.Dequeue();
            direction = transform.position - currentDestination;
            hasPath = true;
        }
        else
        {
            hasPath = false;
        }
    }
 
    // move towards the point
    void MoveAlongPath()
    {
        if(hasPath)
        {
            currentDistance = (transform.position - currentDestination).sqrMagnitude;
            
            if(currentDistance > minDistanceArrived)
            {
           
                transform.position +=  direction * (moveSpeed * Time.deltaTime);
            }
            else
            {
                GetNextCorner();
            }
        }
    }
}
