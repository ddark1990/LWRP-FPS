using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Linq;
using JetBrains.Annotations;
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
    [HideInInspector] public WeaponHolder weaponHolder;
    [HideInInspector] public IKControl iKControl;
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
        weaponHolder = GetComponent<WeaponHolder>();
        iKControl = GetComponent<IKControl>();
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

    private void LookAtTarget()
    {
        // var head = animator.GetBoneTransform(HumanBodyBones.Head);
        // var lookRotation = Quaternion.LookRotation (target.position - head.position);
        // head.rotation *= lookRotation;
        
        
    }
    public Collider[] GetMeleeHitTargets(Collider[] outPutArray, Vector3 hitPoint, float meleeRange, int numOfAllowedHits, LayerMask hitLayer)
    {
        if (outPutArray == null)
            throw new ArgumentNullException(nameof(outPutArray)); //null check on array cuz it is very much so required
        
        outPutArray = new Collider[numOfAllowedHits];
        
        var targetsHit = Physics.OverlapSphereNonAlloc(hitPoint, meleeRange, outPutArray, hitLayer);
        
        return outPutArray;
    }

    [Header("Melee Settings")] 
    public bool meleeDebug;
    public LayerMask hitLayer;
    public float meleeRadiusRange = 0.25f;

    public float sideControl;
    public float heightControl = 1.25f;
    public float distanceControl = 0.7f;
    
    [HideInInspector] public Collider[] targetMeleeArr;

    public void GetMeleeHitTarget() //used by animation events for melee attacks *move into animation events container inside the animation controller for characters
    {
        if(targetMeleeArr.Length == 0)
            targetMeleeArr = new Collider[1];
        
        Physics.OverlapSphereNonAlloc(transform.localPosition + transform.TransformDirection(new Vector3(sideControl,heightControl,distanceControl)), 
            meleeRadiusRange, targetMeleeArr, hitLayer); //do a "non allocating" sphere check
        
        foreach (var hit in targetMeleeArr)
        {
            if (hit != null && hit.transform != transform.root) //if we hit something
            {
                var targetVitals = hit.GetComponent<AiVitals>();

                targetVitals.TakeDamage(10);
                Debug.Log("Hit " + hit.name);
            }
        }
        Array.Clear(targetMeleeArr, 0, 1);
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
    
    private void OnDrawGizmosSelected()
    {
        if (meleeDebug)
        {
            Gizmos.color = new Color(1f, 0.31f, 0.23f);
            Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(new Vector3(sideControl,heightControl,distanceControl)), meleeRadiusRange); //melee sphere range/radius
        }
    }
}
