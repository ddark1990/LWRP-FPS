using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;

    private AudioSource _audioSource;

    [Header("Hitmarker Settings")]
    public AudioClip HitMarkerSound;
    public AudioClip HeadShotSound;
    [Range(0, 1)] public float hitMarkerVolume = 0.2f;

    [Header("Footsteps Settings")]
    public List<FootstepData> footstepData;
    [Space]
    [Range(0, 1)] public float footstepVolume = 0.2f;
    [Header("Bullet Settings")]
    public List<BulletImpactData> bulletImpactData;
    [Space]
    [Range(0,1)] public float bulletImpactVolume = 0.2f;

    [Header("AiDeath Sounds Settings")]
    public new bool enabled;
    public AudioClip[] aiDeathSounds;
    [Range(0, 1)] public float aiVolume = 0.4f;

    private const string HITMARKER = "hitmarker";
    private const string HEADSHOTMARKER = "headshotmarker";


    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
        
        if (!Instance)
            Instance = this;
    }

    public void PlayHitMarker(Collider collider, out float damageMulti) //fix
    {
        var _damageMulti = 1f;

        if (collider.CompareTag(HITMARKER))
        {
            _audioSource.PlayOneShot(HitMarkerSound, hitMarkerVolume);
            _damageMulti = 1f;
        }
        else if (collider.CompareTag(HEADSHOTMARKER))
        {
            _audioSource.PlayOneShot(HeadShotSound, hitMarkerVolume);
            _damageMulti = 1.5f;
        }

        damageMulti = _damageMulti;
    }

    public void PlayDeathSound(AudioSource audioSource)
    {
        if (!enabled) return;

        var rndNum = UnityEngine.Random.Range(0, 4);

        audioSource.PlayOneShot(aiDeathSounds[rndNum], aiVolume);
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
