using UnityEngine;
using System.Collections;

public class MeleeAttackGoapAction : GoapAction
{
    public float attackRate = 1f;

    private float startTime;
    private bool completed;

    public MeleeAttackGoapAction()
    {
        addPrecondition("hasTarget", true);
        addPrecondition("weaponEquiped", false);
        addEffect("meleeAttack", true);
    }

    public override void reset()
    {
        startTime = 0f;
        completed = false;
    }
    
    public override bool isDone()
    {
        return completed;
    }

    public override bool requiresInRange()
    {
        return true;
    }

    public override bool checkProceduralPrecondition(AiStateController controller)
    {
        // if(!controller.target || controller.aiVitals.isDead) return false;
        //
        target = controller.target;

        return true;
    }

    public override bool perform(AiStateController controller)
    {
        if (startTime == 0f)
        {
            Debug.Log("Starting to melee attack.");
            startTime = Time.time;
        }
        if (Time.time - startTime > attackRate /*&& controller.navAgent.remainingDistance <= controller.navAgent.stoppingDistance*/)
        {
            Debug.Log("Finished melee attacking.");
            completed = true;
        }
        return true;
    }
}
