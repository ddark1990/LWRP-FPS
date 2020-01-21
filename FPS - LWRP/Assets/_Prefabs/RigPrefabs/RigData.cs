using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigData : MonoBehaviour
{
    public Transform RightHandSwayHandle;
    public Transform ShootPoint;
    public ParticleSystem MuzzleFlash;

    public Transform OrigPosRef;
    public Transform LookDownSightsRef;
    public Transform ShellDischargePoint;
    public Animator GunAnimator;
}
