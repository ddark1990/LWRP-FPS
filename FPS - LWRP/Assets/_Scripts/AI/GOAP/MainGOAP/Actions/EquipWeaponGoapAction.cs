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
        addPrecondition("weaponEquiped", false);
        addPrecondition("inCombat", true);
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
            
            
            var bestWeapon = controller.aiInventory.GetBestWeaponFromInventory(); //get the best weapon we have in the inventory

            if (bestWeapon.weaponSettings.ranged) //choose the correct animations based on the type of weapon we have equiped
            {
                controller.rangedWeaponEquiped = controller.aiInventory.GetBestWeaponFromInventory();
                
                controller.weaponHolder.ToggleActiveWeapon(controller.rangedWeaponEquiped, true);
                controller.animator.SetTrigger(controller.rangedWeaponEquiped.weaponSettings.animationTriggers.ToString()); //should be pull out rifle, and have a separate for pistols
                controller.animator.SetBool("HasWeaponEquiped", true);
            }
            else
            {
                controller.meleeWeaponEquiped = controller.aiInventory.GetBestWeaponFromInventory();
                
                controller.weaponHolder.ToggleActiveWeapon(controller.meleeWeaponEquiped, true);
                controller.animator.SetTrigger(controller.meleeWeaponEquiped.weaponSettings.animationTriggers.ToString());
                controller.animator.SetBool("HasWeaponEquiped", true);
            }
            

        }

        if ((Time.time - _startTime > controller.animator.GetCurrentAnimatorStateInfo(2).length)) //wait til animation is over
        {
            //Debug.Log("Finished equipping weapon.");

            controller.iKControl.enableHandIk = true;
            
            _completed = true;
        }
       
        return true;
    }
}
