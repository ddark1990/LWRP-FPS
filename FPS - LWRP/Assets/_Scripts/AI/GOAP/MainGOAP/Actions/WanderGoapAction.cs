using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class WanderGoapAction : GoapAction
{
    public float waitMinTime = 1f;
    public float waitMaxTime = 1f;
    public float wanderRadius = 5f;

    public bool showWanderRadius;
    
    private float _tempWaitTime;
    private float _elapsedTime;
    private NavMeshPath _path;
    private bool _completed;

    public WanderGoapAction()
    {
        addPrecondition("pickUpAvailable", false);
        addPrecondition("inCombat", false);
        addEffect("wander", true);
    }

    public override void reset()
    {
        _tempWaitTime = Random.Range(waitMinTime, waitMaxTime);
        _elapsedTime = 0f;
        _completed = false;
    }

    public override bool isDone()
    {
        return _completed;
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override bool checkProceduralPrecondition(AiStateController controller)
    {
        return true;
    }

    public override bool perform(AiStateController controller)
    {
        if (controller.pickUpAvailable) return false;
        
        if (_elapsedTime == 0 && !controller.navAgent.hasPath)
        {
            //Debug.Log("Starting to wander.");

            _path = new NavMeshPath();

            NavMesh.CalculatePath(controller.navAgent.transform.position, controller.GetRandomRadialPos(wanderRadius), NavMesh.AllAreas, _path);
            //Debug.Log("Path Calculated for " + agent.name);

            controller.navAgent.SetPath(_path);
            //Debug.Log("Path Set for " + agent.name);
        }
        if (controller.navAgent.remainingDistance <= controller.navAgent.stoppingDistance)
        {
            //Debug.Log("Started waiting.");
            _elapsedTime += Time.deltaTime;

            if(_elapsedTime >= _tempWaitTime)
            {
                //Debug.Log("Finished waiting.");
                controller.navAgent.ResetPath();
                _completed = true;
            }
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        if (!showWanderRadius) return;
        {
            var color = Color.Lerp(Color.yellow, Color.magenta, Mathf.PingPong(Time.time, 1));

            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, wanderRadius);
        }
    }
}
