using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoomerFPSController;

public class WeaponRecoil : MonoBehaviour
{
    public static WeaponRecoil Instance;

    [Header("Cache")]
    public WeaponManager WeaponManager;
    public Transform CameraHolder;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    public void ApplyGunRecoil(float recoilUpKick, float recoilSideKick)
    {
        FPSController.Instance.desiredPitch -= recoilUpKick; //upkick
        FPSController.Instance.desiredYaw += recoilSideKick; //sidekick
    }
}
