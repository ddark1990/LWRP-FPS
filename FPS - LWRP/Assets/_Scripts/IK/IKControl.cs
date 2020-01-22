using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class IKControl : MonoBehaviour
{
    public bool enableFeetIk = true;
    public bool enableHandIk = true;
    
    [Header("HandIK")] 
    public Transform leftHandIkPosition;
    public Transform rightHandIkPosition;
    [Header("FootIK")]
    public float distanceToGround = 0.1f;
    public LayerMask groundLayer;
    
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!_animator) return;

        if (enableHandIk)
        {
            //left hand
            if (leftHandIkPosition)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

                _animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIkPosition.position);
                _animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIkPosition.rotation);
            }
            //right hand
            if (rightHandIkPosition)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);

                _animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIkPosition.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIkPosition.rotation);
            }
        }

        if (enableFeetIk)
        {
            //set feet weights
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);

            //left foot
            RaycastHit hit;
            Ray ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out hit, distanceToGround + 1f, groundLayer))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    var footPos = hit.point;
                    footPos.y += distanceToGround;
                    
                    _animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);
                    _animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation);
                }
            }
            
            //right foot
            ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out hit, distanceToGround + 1f, groundLayer))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    var footPos = hit.point;
                    footPos.y += distanceToGround;
                    
                    _animator.SetIKPosition(AvatarIKGoal.RightFoot, footPos);
                    _animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation);
                }
            }

        }
    }
}
