using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AiAction : ScriptableObject
{
    public abstract void Act(AiStateController controller, NavMeshAgent agent);
}
