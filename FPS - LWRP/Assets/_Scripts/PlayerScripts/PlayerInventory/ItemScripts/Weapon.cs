using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "WeaponItem", menuName = "Create Item/Item Type/Weapon", order = 1)]
public class Weapon : ItemType
{
    
    [Serializable]
    public struct WeaponSettings
    {
        public enum AnimationTriggers
        {
            PullOutPistol,
            PullOutRifle,
            PullOutMeleeWeapon
        }

        public bool ranged;
        [Range(0,3)] public byte weaponTier;
        
        [Header("Rig Settings")]
        public GameObject RigPrefab;
        
        [Header("Melee Settings")] 
        public float meleeDamage;
        public float meleeAttackRate;
        
        [Header("Ranged Settings")]
        public bool Automatic;
        [Range(0, 10)] public float ReloadTime;
        [Range(0, 20)] public float RecoilUpKick;
        [Range(0, 20)] public float RecoilSideKick;
        [Range(0.1f, 25)] public float FireRate;
        [Range(250f, 5000f)] public float ForceMultiplier;

        [Header("Bullet Settings")]
        public GameObject BulletPrefab;
        public GameObject ShellPrefab;
        [Range(1, 100)] public int MaxClipAmmo;
        [Range(1, 100)] public int BulletDamage;
        [Range(0.1f, 10f)] public float BulletDrop;

        [Header("Sound Settings")]
        public AudioClip EquipSound;
        public AudioClip[] ShootSounds;
        public AudioClip ReloadSound;

        [Header("Animation Settings")]
        
        public AnimationTriggers animationTriggers; //could take the manual route and type a string in, or have a enum dropdown for convenience 
        //public string animationTrigger;

    }

    public WeaponSettings weaponSettings;
}
