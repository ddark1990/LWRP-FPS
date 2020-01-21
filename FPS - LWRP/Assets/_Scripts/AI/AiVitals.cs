using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class AiVitals : MonoBehaviour
{
    [SerializeField] private float Health;
    public bool isDead;

    [Header("Cache")]
    public RagdollController ragDollController;

    private float startingHealth;

    private void Awake()
    {
        startingHealth = Health;
    }

    private void FixedUpdate()
    {
        if (!Input.GetKeyDown(KeyCode.P)) return;
        Die();
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = !isDead;

        //Debug.Log(gameObject.name + " died!");
        var animator = GetComponent<Animator>();
        var navAgent = GetComponent<NavMeshAgent>();
        var rb = GetComponent<Rigidbody>();
        var lastColliderHitName = WeaponManager.Instance.ColliderHit.name;
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

        //ragDollController.ApplyBulletForceToCollider(lastColliderHitName);
    }

}
