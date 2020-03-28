using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Ai/Actions/Attack")]
public class AttackTargetAction : AiAction
{
    public override void Act(AiController controller, NavMeshAgent agent)
    {
        AttackTarget(agent);
    }

    private void AttackTarget(NavMeshAgent agent)
    {
        agent.isStopped = true;

    }

}
