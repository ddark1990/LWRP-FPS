using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EquipWeaponGoapAction : GoapAction
{
    private float _startTime;
    private bool _completed;
    
    private static readonly int hasWeaponEquiped = Animator.StringToHash("HasWeaponEquipped");
    private static readonly int equipPistol = Animator.StringToHash("EquipPistol");
    private static readonly int equipRifle = Animator.StringToHash("EquipRifle");
    private static readonly int equipMeleeWeapon = Animator.StringToHash("EquipMeleeWeapon");

    public EquipWeaponGoapAction()
    {
        addPrecondition("hasWeaponInInventory", true);
        addPrecondition("weaponEquipped", false);
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
            
            var bestWeapon = controller.aiInventory.GetBestWeaponFromInventory(); //get the best weapon based on a weapon tier check
            
            if (bestWeapon.weaponSettings.ranged) //choose the correct animations based on the type of weapon we have equipped
            {
                controller.rangedWeaponEquiped = controller.aiInventory.GetBestWeaponFromInventory();
                
                if (controller.rangedWeaponEquiped.weaponSettings.weaponTags == Weapon.WeaponSettings.WeaponTags.Pistol)
                {
                    controller.weaponHolder.ToggleActiveWeapon(controller.rangedWeaponEquiped, true);
                    controller.animator.SetTrigger(equipPistol); 
                    controller.animator.SetBool(hasWeaponEquiped, true);
                }
                else if (controller.rangedWeaponEquiped.weaponSettings.weaponTags == Weapon.WeaponSettings.WeaponTags.Rifle)
                {
                    controller.weaponHolder.ToggleActiveWeapon(controller.rangedWeaponEquiped, true);
                    controller.animator.SetTrigger(equipRifle); 
                    controller.animator.SetBool(hasWeaponEquiped, true);
                }
            }
            else
            {
                controller.meleeWeaponEquiped = controller.aiInventory.GetBestWeaponFromInventory();
                
                controller.weaponHolder.ToggleActiveWeapon(controller.meleeWeaponEquiped, true);
                controller.animator.SetTrigger(equipMeleeWeapon);
                controller.animator.SetBool(hasWeaponEquiped, true);
            }
        }

        if ((Time.time - _startTime > controller.animator.GetCurrentAnimatorStateInfo(2).length)) //wait til animation is over
        {
            //Debug.Log("Finished equipping weapon.");

            //controller.iKControl.enableHandIk = true; //corrects the hand ik
            
            _completed = true;
        }
       
        return true;
    }
}
