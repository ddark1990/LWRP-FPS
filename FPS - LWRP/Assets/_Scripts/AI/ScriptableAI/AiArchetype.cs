using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using static AiStateController;

[CreateAssetMenu(menuName = "Ai/New Archetype", fileName = "New Ai Archetype")]
public class AiArchetype : ScriptableObject
{
    public FieldOfViewData fovData;
    public CombatData combatData;
    
    [Serializable]
    public struct FieldOfViewData
    {
        [Range(0, 360)] public float maxAgroRadius;
        [Range(0, 360)] public float maxViewAngle;
    }

    [Serializable]
    public struct CombatData
    {
        [Range(0, 9999)] public float meleeDamage;
        [Range(0, 2)] public float meleeDistance;
    }
}
