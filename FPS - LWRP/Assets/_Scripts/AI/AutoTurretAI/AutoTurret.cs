using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTurret : MonoBehaviour
{
    public bool isActive;

    [Header("Cache")]
    public AudioSource audioSource;
    public AiController aiController;
    public Animator animator;
    public Transform shaftTransform;
    private static readonly int HasTarget = Animator.StringToHash("hasTarget");
    private static readonly int Active = Animator.StringToHash("Active");

    private void Update()
    {
        SetAnimationParams(animator);
        //LookAtTarget(aiStateController.target);
    }

    private void LookAtTarget(Transform target)
    {
        if (!target) return;

        shaftTransform.LookAt(target.position);
    }

    private void SetAnimationParams(Animator anim)
    {
        //anim.SetBool(HasTarget, aiStateController.target);
        anim.SetBool(Active, isActive);
    }
}
