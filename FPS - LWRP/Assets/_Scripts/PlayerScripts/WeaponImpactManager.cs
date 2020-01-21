using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EffectsManager;

public class WeaponImpactManager : MonoBehaviour
{
    public static void PlayImpactEffect(string tag, Vector3 impactPoint, Vector3 norm, List<BulletImpactData> impactDataList)
    {
        CreateImpactEffect(tag, impactPoint, norm, impactDataList);
    }

    private static void CreateImpactEffect(string tag, Vector3 impactPoint, Vector3 impactPointNormal, List<BulletImpactData> impactDataList)
    {
        for (int i = 0; i < impactDataList.Count; i++)
        {
            var data = impactDataList[i];

            if (data.TagName.Equals(tag))
            {
                var impactFx = Instantiate(data.ImpactEffect, impactPoint, Quaternion.LookRotation(impactPointNormal));
            }
        }
    }
}