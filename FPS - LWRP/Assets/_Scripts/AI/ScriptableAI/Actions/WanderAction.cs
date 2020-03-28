using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Ai/Actions/Wander")]
public class WanderAction : AiAction
{
    //public float WanderRadius;
    //public float WaitTime = 5;

    NavMeshPath path;
    float elapsedTime = 0f;

    public override void Act(AiController controller, NavMeshAgent agent)
    {
        //Wander(controller, agent, controller.aiArchetype.wanderData.WaitTime, controller.aiArchetype.wanderData.RadiusSizes.y);
    }

    public void Wander(AiController controller, NavMeshAgent agent, float waitTime, float wanderRadius)
    {
        if (agent.hasPath) return;

        path = new NavMeshPath();

        elapsedTime += Time.deltaTime;
        //Debug.Log(elapsedTime);

        if (elapsedTime > waitTime)
        {
            elapsedTime -= waitTime;
            NavMesh.CalculatePath(agent.transform.position, controller.GetRandomRadialPos(wanderRadius), NavMesh.AllAreas, path);
            //Debug.Log("Path Calculated for " + agent.name);

            agent.SetPath(path);
            //Debug.Log("Path Set for " + agent.name);
        }
    }
}