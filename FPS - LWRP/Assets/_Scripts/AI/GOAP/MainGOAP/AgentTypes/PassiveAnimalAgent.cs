/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PassiveAnimalAgent : GoapAgent, IGoap
{
    private Vector3 previousDestination;

    private void OnEnable()
    {
        aiStateControler = GetComponent<AiStateController>(); //get main AI controller
        InitializeAgent();
    }

    /**
	 * Key-Value data that will feed the GOAP actions and system while planning.
	 #1#
    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        worldData.Add(new KeyValuePair<string, object>("hasTarget", !aiStateControler.Target.Equals(null)));
        //worldData.Add(new KeyValuePair<string, object>("hasFood", aiStateControler.Target.gameObject.layer.Equals(LayerMask.NameToLayer("Vegetation"))));

        return worldData;
    }

    public HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        //goal.Add(new KeyValuePair<string, object>("eat", true));

        return goal;
    }


    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
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

        aiStateControler.navAgent.SetDestination(nextAction.target.transform.position);

        if (aiStateControler.navAgent.hasPath && aiStateControler.navAgent.remainingDistance <= aiStateControler.navAgent.stoppingDistance)
        {
            nextAction.setInRange(true);
            previousDestination = nextAction.target.transform.position;
            return true;
        }
        else
            return false;
    }
}
*/
