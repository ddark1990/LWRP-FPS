using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Vitals : MonoBehaviour
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

    private float _startingHealth;

    private const float MaxHunger = 50;
    private const float MaxThirst = 100;

    private AiController _aiController;
    
    
    private void Awake()
    {
        _startingHealth = health;
        _aiController = GetComponent<AiController>();
    }
    
    private void Update()
    {
        HungerOutput();
        ThirstOutput();
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
        
        hunger -= Time.deltaTime * hungerRate;
    }
    private void ThirstOutput()
    {
        if (thirst <= 0) return;
        
        thirst -= Time.deltaTime * thirstRate;
    }
    
    private void Die()
    {
        isDead = true;

        GameManager.Instance.OnKilledAi(_aiController);
        
        //EffectsManager.Instance.PlayDeathSound(audioSource); //needs audio source
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
