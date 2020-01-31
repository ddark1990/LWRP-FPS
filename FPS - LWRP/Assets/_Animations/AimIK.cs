using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

//Rotates the chest to the target ignoring the X value when moving closer to target so he doesnt rotate the chest down
public class AimIK : MonoBehaviour
{
    public Transform targetIk;
    public Vector3 rotationOffset;
    
    private Animator _anim;
    private Transform _chest;
    
    private void Start()
    {
        _anim = GetComponent<Animator>();
        _chest = _anim.GetBoneTransform(HumanBodyBones.Chest);
    }

    private void LateUpdate()
    {
        if (!targetIk) return;
        
        var lookAtPosition = new Vector3(targetIk.position.x, _chest.position.y, targetIk.position.z);
        
        _chest.LookAt(lookAtPosition);
        _chest.rotation *= Quaternion.Euler(rotationOffset);
    }
}
