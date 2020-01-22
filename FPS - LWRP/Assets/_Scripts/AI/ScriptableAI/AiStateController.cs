﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Linq;
using NewAISystem;
using UnityEngine.Experimental;
using UnityEngine.Experimental.AI;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Jobs;
using static AiJobUtil;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(FieldOfView))]
[RequireComponent(typeof(AiVitals))]
public class AiStateController : MonoBehaviour
{
    public Transform target;
    public Weapon weaponEquiped;
    
    public bool hasAgro, isFeeding, isResting, isFleeing, isIdle, isHungry;
    //goap action states
    public bool pickUpAvailable, inCombat;

    public float distanceFromTarget;

    [HideInInspector] public AiVitals aiVitals;
    [HideInInspector] public FieldOfView fieldOfView;
    [HideInInspector] public GenericInventory aiInventory;
    [HideInInspector] public NavMeshAgent navAgent;
    [HideInInspector] public Animator animator; //create an animator controller

    [Header("Debug")] 
    public bool drawDebugPathLines;
    //private
    private JobManager _jobManager;
    private static readonly int InCombat = Animator.StringToHash("InCombat");
    
    #region UnityFunctions
    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    #endregion
    
    private IEnumerator Init()
    {
        GetComponents();
        yield return new WaitForEndOfFrame();
        _jobManager.InitializeAi(this);
    }
    
    private void GetComponents()
    {
        _jobManager = FindObjectOfType<JobManager>();
        aiVitals = GetComponent<AiVitals>();
        fieldOfView = GetComponent<FieldOfView>();
        aiInventory = GetComponentInChildren<GenericInventory>();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!drawDebugPathLines) return;
        DrawCornerLines(navAgent.path.corners);
    }
    
    private void DrawCornerLines(IReadOnlyList<Vector3> corners)
    {
        for (int i = 0; i < corners.Count; i++)
        {
            var corner = corners[i];

            if ((i + 1) < corners.Count)
            {
                Debug.DrawLine(corner, corners[i + 1], Color.green);
            }
        }
    }
    private void SetAgro(bool state)
    {
        hasAgro = state;
    }
    private void SetCombatState(bool state)
    {
        inCombat = state;
        SetAgro(state);

        InitializeAnimationParameters(animator, state);
    }
    private static void InitializeAnimationParameters(Animator anim, bool state)
    {
        anim.SetBool(InCombat, state);
    }

    #region HelperFunctions
    
    public Vector3 GetRandomRadialPos(float radiusSize)
    {
        var randomPos = transform.position + OnUnitSphere() * radiusSize;
        return randomPos;
    }

    private static Vector3 OnUnitSphere() //move into job
    {
        return new Vector3(UnityEngine.Random.insideUnitCircle.x, 0, UnityEngine.Random.insideUnitCircle.y);
    }

    public Item FindClosestItemInCollection(Collider[] visibleTargets, Transform compareTo) // jobify
    {
        if (visibleTargets.Length == 0) return null;
        
        Item closestItem = null;
        var closestDist = 0f;
        
        foreach (var tempCollider in visibleTargets)
        {
            if (!tempCollider) break;
            
            if (tempCollider.GetComponent<Item>() && closestItem == null) 
            {
                // first one, so choose it for now
                closestItem = tempCollider.GetComponent<Item>();
                closestDist = (tempCollider.transform.position - compareTo.position).magnitude;
                
            } 
            else if (tempCollider.GetComponent<Item>())
            {
                // is this one closer than the last?
                var dist = (tempCollider.transform.position - compareTo.position).magnitude;

                if (!(dist < closestDist)) continue;
                
                // we found a closer one, use it
                closestItem = tempCollider.GetComponent<Item>();
                closestDist = dist;
            }
        }
        
        return closestItem;
    }
    #endregion
    
}