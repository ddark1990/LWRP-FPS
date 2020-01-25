using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PickUpItemGoapAction : GoapAction
{
    public float extraWaitTime = 0.5f;
    
    private float _startTime;
    private bool _completed;
    private Item _targetItem;

    public PickUpItemGoapAction()
    {
        addPrecondition("pickUpAvailable", true);
        addEffect("pickUpItem", true);
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
        return true;
    }

    public override bool checkProceduralPrecondition(AiStateController controller)
    {
        if (!controller.pickUpAvailable) return false;

        var closestItem = controller.FindClosestItemInCollection(controller.fieldOfView.resultTargetArr, transform);
        
        target = closestItem.transform;
        
        return true;
    }
    
    public override bool perform(AiStateController controller)
    {
        Debug.Log(controller.animator.GetCurrentAnimatorStateInfo(1).length);
        if (_startTime == 0)
        {
            _targetItem = target.GetComponent<Item>();

            //Debug.Log("Starting to pick up item.");
            
            _startTime = Time.time;
            controller.animator.SetTrigger("PickUpItem");
        }

        if ((Time.time - _startTime > controller.animator.GetCurrentAnimatorStateInfo(1).length + extraWaitTime)) //wait til animation is over
        {
            //Debug.Log("Picked up item.");
        
            controller.aiInventory.AddItemToInventory(_targetItem);
            
            controller.pickUpAvailable = false; //reset 
            _completed = true;
        }
       
        return true;
    }
}
