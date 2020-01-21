using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class AgressiveHumanoidAgent : GoapAgent, IGoap
{
    private Vector3 previousDestination;
    private HashSet<KeyValuePair<string, object>> worldData;
    private HashSet<KeyValuePair<string, object>> goalData;
    
    private void OnEnable()
    {
        aiStateController = GetComponent<AiStateController>(); //get main AI controller
        InitializeAgent();
    }
    
    /**
	 * Key-Value data that will feed the GOAP actions and system while planning.
	 */
    public HashSet<KeyValuePair<string,object>> getWorldState()
    {
        worldData = new HashSet<KeyValuePair<string, object>>();

        worldData.Add(new KeyValuePair<string, object>("pickUpAvailable", aiStateController.pickUpAvailable));
        worldData.Add(new KeyValuePair<string, object>("inCombat", aiStateController.inCombat));
        worldData.Add(new KeyValuePair<string, object>("holdsWeapon", aiStateController.aiInventory.HasWeaponsInInventory()));
        
        return worldData;
    }

    public HashSet<KeyValuePair<string, object>> createGoalState()
    {
        goalData = new HashSet<KeyValuePair<string, object>>( );

        goalData.Add(new KeyValuePair<string, object>("wander", true));
        goalData.Add(new KeyValuePair<string, object>("pickUpItem", true));
        goalData.Add(new KeyValuePair<string, object>("equipWeapon", true));
        
        return goalData;
    }
    
    public void planFailed(HashSet<KeyValuePair<string,object>> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
        Debug.Log("failedGoal");
    }

    public void planFound(HashSet<KeyValuePair<string,object>> goal, Queue<GoapAction> actions)
    {
        // Yay we found a plan for our goal
        //Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
    }

    public void actionsFinished()
    {
        // Everything is done, we completed our actions for this gool. Hooray!
        //Debug.Log("<color=blue>Actions completed</color>");
    }

    public void planAborted(GoapAction aborter)
    {
        // An action bailed out of the plan. State has been reset to plan again.
        // Take note of what happened and make sure if you run the same goal again
        // that it can succeed.
        //Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
    }

    public bool moveAgent(GoapAction nextAction)
    {
         //if we don't need to move anywhere
         if (previousDestination == nextAction.target.transform.position)
         {
             nextAction.setInRange(true);
             return true;
         }

        aiStateController.navAgent.SetDestination(nextAction.target.transform.position);

        if (aiStateController.navAgent.hasPath && !aiStateController.navAgent.pathPending && aiStateController.navAgent.remainingDistance <= aiStateController.navAgent.stoppingDistance)
        {
            nextAction.setInRange(true);
            previousDestination = nextAction.target.transform.position;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void FixedUpdate()
    {
        // if (aiStateController.navAgent.hasPath)
        // {
        //     var toTarget = aiStateController.navAgent.steeringTarget - transform.position;
        //     var turnAngle = Vector3.Angle(transform.forward, toTarget);
        //     aiStateController.navAgent.acceleration = turnAngle * aiStateController.navAgent.speed;
        // }
    }
}