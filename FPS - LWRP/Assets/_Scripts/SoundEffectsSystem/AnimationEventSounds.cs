using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSounds : MonoBehaviour
{
    [Header("Footstep Settings")]
    public bool enableStepSounds;

    [Header("Ground Check Settings")]
    [Tooltip("How high, relative to the character's pivot point the start of the ray is.")]
    [SerializeField] float groundCheckHeight = 0.5f;
    [Tooltip("What is the radius of the ray.")]
    [SerializeField] float groundCheckRadius = 0.5f;
    [Tooltip("How far the ray is casted.")]
    [SerializeField] float groundCheckDistance = 0.3f;
    [Tooltip("What are the layers that should be taken into account when checking for ground.")]
    [SerializeField] LayerMask groundLayers;

    [Header("Cache")]
    public AudioSource audioSource;

    private RaycastHit currentHitInfo;

    private void StepSound()
    {
        if (!enableStepSounds) return;

        AudioClip clip = GetRandomTerrainClip();
        audioSource.PlayOneShot(clip, EffectsManager.Instance.FootstepVolume);
    }

    private AudioClip GetRandomTerrainClip()
    {
        int terrainTextureIndex = TextureDetector.GetActiveTerrainTextureIdx(transform.position);
        var footstepData = EffectsManager.Instance.footstepData;

        for (int i = 0; i < footstepData.Count; i++)
        {
            var a = footstepData[i];

            if (a.index.Equals(terrainTextureIndex))
            {
                var clip = footstepData[i].FootstepSounds[Random.Range(0, footstepData[i].FootstepSounds.Length)];

                return clip;
            }
        }

        return null;/*footstepData[terrainTextureIndex].FootstepSounds[Random.Range(0, footstepData[terrainTextureIndex].FootstepSounds.Length)];*/
    }


}
