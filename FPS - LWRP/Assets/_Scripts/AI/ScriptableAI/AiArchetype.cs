using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using static AiController;

[CreateAssetMenu(menuName = "Ai/New Archetype", fileName = "New Ai Archetype")]
public class AiArchetype : ScriptableObject
{
    public FieldOfViewSettings fovSettings;
    public CombatSettings combatSettings;
    
    [Serializable]
    public struct FieldOfViewSettings
    {
        [Range(0, 360)] public float maxAgroRadius;
        [Range(0, 360)] public float maxViewAngle;
    }

    [Serializable]
    public struct CombatSettings
    {
        [Range(0, 9999)] public float meleeDamage;
        [Range(0, 5)] public float meleeDistance;
    }
}
