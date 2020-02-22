using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class AiVitals : MonoBehaviour
{
    public float health;
    public float hunger = 100;
    public float thirst = 100;
    
    [Header("Vitals Reduce Rate")]
    [SerializeField] private float hungerRate = 1;
    [SerializeField] private float thirstRate = 1;
    
    [Header("Hungry/Thirsty At")]
    [SerializeField] private float hungerThreshold = 25;
    [SerializeField] private float thirstThreshold = 1;
    
    public bool isDead;

    [Header("Cache")]
    public RagdollController ragDollController;

    private float _startingHealth;

    private const float MaxHunger = 50;
    private const float MaxThirst = 100;

    private AiStateController _aiStateController;
    
    
    private void Awake()
    {
        _startingHealth = health;
        _aiStateController = GetComponent<AiStateController>();
    }
    
    private void Update()
    {
        HungerOutput();
        ThirstOutput();
    }

    private void FixedUpdate()
    {
        if (!Input.GetKeyDown(KeyCode.P)) return;
        Die();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !isDead)
        {
            Die();
        }
    }
    
    private void HungerOutput()
    {
        if (hunger <= 0) return;
        //hunger = Mathf.Clamp(hunger, 0, MaxHunger);
        hunger -= Time.deltaTime * hungerRate;
    }
    private void ThirstOutput()
    {
        if (thirst <= 0) return;
        //thirst = Mathf.Clamp(thirst, 0, MaxThirst);
        thirst -= Time.deltaTime * thirstRate;
    }
    
    private void Die()
    {
        isDead = !isDead;

        //Debug.Log(gameObject.name + " died!");
        var animator = GetComponent<Animator>();
        var navAgent = GetComponent<NavMeshAgent>();
        var rb = GetComponent<Rigidbody>();
        //var lastColliderHitName = WeaponManager.Instance.ColliderHit.name;
        //Debug.Log(lastColliderHitName);

        if (isDead) //turn into interface
        {
            RagdollController.CopyTransformData(ragDollController.mainModel, ragDollController.ragdoll, animator.velocity); 
            ragDollController.SetRadDollState(this, animator, navAgent, rb, true);
        }
        else
        {
            ragDollController.SetRadDollState(this, animator, navAgent, rb, false);
        }

        GameManager.instance.OnKilledAi(_aiStateController);

        //ragDollController.ApplyBulletForceToCollider(lastColliderHitName);
    }

    public bool IsHungry()
    {
        return hunger <= hungerThreshold;
    }
    public bool IsThirsty()
    {
        return thirst <= thirstThreshold;
    }
}
