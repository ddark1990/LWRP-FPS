using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;

    public AudioSource AudioSource;

    [Header("Hitmarker Settings")]
    public AudioClip HitMarkerSound;
    public AudioClip HeadShotSound;
    [Range(0, 1)] public float hitMarkerVolume = 0.2f;

    [Header("Footsteps Settings")]
    public List<FootstepData> footstepData;
    [Space]
    [Range(0, 1)] public float FootstepVolume = 0.2f;
    [Header("Bullet Settings")]
    public List<BulletImpactData> bulletImpactData;
    [Space]
    [Range(0,1)] public float BulletImpactVolume = 0.2f;

    [Header("AiDeath Sounds Settings")]
    public bool enabled;
    public AudioClip[] AiDeathSounds;
    [Range(0, 1)] public float AiVolume = 0.4f;

    private const string HITMARKER = "hitmarker";
    private const string HEADSHOTMARKER = "headshotmarker";


    private void OnEnable()
    {
        if (!Instance)
            Instance = this;
    }

    public void PlayHitMarker(Collider collider, out float damageMulti)
    {
        var _damageMulti = 1f;

        if (collider.CompareTag(HITMARKER))
        {
            AudioSource.PlayOneShot(HitMarkerSound, hitMarkerVolume);
            _damageMulti = 1f;
        }
        else if (collider.CompareTag(HEADSHOTMARKER))
        {
            AudioSource.PlayOneShot(HeadShotSound, hitMarkerVolume);
            _damageMulti = 1.5f;
        }

        damageMulti = _damageMulti;
    }

    public void PlayDeathSound(AudioSource audioSource)
    {
        if (!enabled) return;

        var rndNum = UnityEngine.Random.Range(0, 4);

        audioSource.PlayOneShot(AiDeathSounds[rndNum], AiVolume);
    }

    [Serializable]
    public struct FootstepData
    {
        public string Name;
        public int index;
        public AudioClip[] FootstepSounds;
        public ParticleSystem FootstepEffects;
    }

    [Serializable]
    public struct BulletImpactData
    {
        public string TagName;
        public AudioClip[] ImpactSounds;
        public GameObject ImpactEffect;
    }
}
