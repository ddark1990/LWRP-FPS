using System;
using UnityEngine;
using System.Collections;

public class MeleeAttackGoapAction : GoapAction
{
    public float attackRate = 1f;
    public float damageDelay = 0.1f;
    
    private float startTime;
    private bool completed;
    private static readonly int meleeAttack = Animator.StringToHash("MeleeAttack");
    private static readonly int combatLayer = Animator.StringToHash("CombatLayer");

    public MeleeAttackGoapAction()
    {
        addPrecondition("hasTarget", true);
        //addPrecondition("weaponEquiped", false);
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

    public override bool checkProceduralPrecondition(AiController controller)
    {
        if(!controller.target || controller.vitals.isDead) return false;

        controller.navAgent.stoppingDistance = controller.aiArchetype.combatSettings.meleeDistance;
        
        target = controller.target;
        return true;
    }
    
    public override bool perform(AiController controller) //activates animation which holds an event that gives all hits
    {
        if (startTime == 0f)
        {
            Debug.Log("Starting to melee attack.");
            
            controller.animator.SetTrigger(meleeAttack);
            
            startTime = Time.time;
        }
        if (Time.time - startTime > attackRate)
        {
            Debug.Log("Finished melee attacking.");

            controller.navAgent.stoppingDistance = 0.2f; //reset stopping distance to default
            
            completed = true;
        }
        return true;
    }
    
}
