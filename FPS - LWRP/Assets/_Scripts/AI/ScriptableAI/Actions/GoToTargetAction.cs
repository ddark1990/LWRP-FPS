using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Ai/Actions/Follow")]
public class GoToTargetAction : AiAction
{
    public override void Act(AiController controller, NavMeshAgent agent)
    {
        GoToTarget(controller, agent);
    }

    private void GoToTarget(AiController controller, NavMeshAgent agent)
    {
        agent.isStopped = false;

        //agent.SetDestination(controller.target.position);
    }

}
