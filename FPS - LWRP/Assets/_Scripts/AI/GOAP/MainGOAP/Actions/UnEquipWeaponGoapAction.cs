using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class UnEquipWeaponGoapAction : GoapAction
{
    private float _startTime;
    private bool _completed;

    public UnEquipWeaponGoapAction()
    {
        addPrecondition("weaponEquiped", true);
        addPrecondition("inCombat", false);
        addEffect("unEquipWeapon", true);
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

    public override bool checkProceduralPrecondition(AiController controller)
    {
        return true;
    }

    public override bool perform(AiController controller)
    {
        if (_startTime == 0)
        {
            //Debug.Log("Starting to un equip weapon.");
            _startTime = Time.time;
            
            controller.iKControl.enableHandIk = false;

            controller.animator.SetTrigger("PutAwayWeapon");
            controller.animator.SetBool("HasWeaponEquiped", false);
            
            //controller.weaponEquiped = controller.aiInventory.GetBestWeaponFromInventory();
        }

        if ((Time.time - _startTime > controller.animator.GetCurrentAnimatorStateInfo(2).length)) //wait til animation is over
        {
            //Debug.Log("Finished un equipping weapon.");

            controller.weaponHolder.ToggleActiveWeapon(controller.rangedWeaponEquiped, false);
            controller.rangedWeaponEquiped = null;

            _completed = true;
        }
       
        return true;
    }
}
