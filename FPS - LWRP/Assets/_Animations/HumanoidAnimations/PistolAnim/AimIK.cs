using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AimIK : MonoBehaviour
{
    public Transform targetIk;
    public Vector3 offSetVector;
    public float damping = 1;

    private Animator _anim;
    private Transform _chest;
    private Quaternion _chestRotation;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _chest = _anim.GetBoneTransform(HumanBodyBones.Chest);
    }

    private void LateUpdate()
    {
        if (!targetIk) return;

        var position = targetIk.position;
        
        _chest.LookAt(new Vector3(position.x, _chest.position.y, position.z));
        _chest.rotation *= Quaternion.Euler(offSetVector);
        
    }
}
