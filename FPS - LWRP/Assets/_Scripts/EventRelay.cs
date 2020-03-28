using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class EventRelay 
{
    //target
    public static event Action<Transform> OnSeenTarget;
    public static event Action<Transform> LostVisionOfTarget;
    public static event Action<Transform> OnKilledTarget;
    public static event Action<AiController> OnKilledAi;
    //item
    public static event Action<Item> OnPickedUpItem;
    public static event Action<Item> OnDropedItem;
    public static event Action<Item> OnUseItem;
    //player scoring
    public static event Action<Collider> OnBodyPartHit;

}
