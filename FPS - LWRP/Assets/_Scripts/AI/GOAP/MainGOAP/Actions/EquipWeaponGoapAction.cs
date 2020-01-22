using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EquipWeaponGoapAction : GoapAction
{
    private float _startTime;
    private bool _completed;

    public EquipWeaponGoapAction()
    {
        addPrecondition("hasWeaponInInventory", true);
        addPrecondition("weaponEquipAvailable", true);
        addEffect("equipWeapon", true);
    }

    public override void reset()
    {
        _startTime = 0f;
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
        if (_startTime == 0)
        {
            //Debug.Log("Starting to equip weapon.");
            _startTime = Time.time;
            
            controller.animator.SetTrigger("PullOutWeapon");
            controller.animator.SetBool("HasWeaponEquiped", true);
        }

        if ((Time.time - _startTime > controller.animator.GetCurrentAnimatorStateInfo(2).length)) //wait til animation is over
        {
            //Debug.Log("Finished equipping weapon.");
            
            controller.weaponEquiped = controller.aiInventory.GetBestWeaponFromInventory();
            
            _completed = true;
        }
       
        return true;
    }
}
