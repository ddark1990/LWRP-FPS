using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using static AiStateController;

[CreateAssetMenu(menuName = "Ai/New Archetype", fileName = "New Ai Archetype")]
public class AiArchetype : ScriptableObject
{
    public enum BehaviourType { Passive, Neutral, Agressive }
    public BehaviourType behaviourType;

    public WanderData wanderData;
    public FieldOfViewData fovData;
    public CombatData combatData;
    public AiAbleData ableData;

    [Serializable]
    public struct WanderData
    {
        [Tooltip("X = Small, Y = Medium, Z = Large")] public float3 RadiusSizes;
        public int WaitTime;
    }

    [Serializable]
    public struct FieldOfViewData
    {
        [Range(0, 360)] public float MaxAgroRadius;
        [Range(0, 360)] public float MaxViewAngle;
    }

    [Serializable]
    public struct CombatData
    {
        [Range(0, 9999)] public float MeleeDamage;
        [Range(0, 2)] public float MeleeDistance;
    }

    [Serializable]
    public struct AiAbleData
    {
        public bool AbleToFeed, AbleToRest;
    }
}
