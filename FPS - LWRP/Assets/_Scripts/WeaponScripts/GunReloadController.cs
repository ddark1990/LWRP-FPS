using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunReloadController : MonoBehaviour
{
    public int currentHoldingAmmo;
    public int maxClipAmmo;
    public bool isEmpty;
    public bool isReloading;

    private Weapon weaponType;

    private void Start()
    {
        weaponType = GetComponent<Item>().ItemType as Weapon;

        maxClipAmmo = weaponType.weaponSettings.MaxClipAmmo;

        currentHoldingAmmo = maxClipAmmo;
    }

    public IEnumerator Reload(float reloadTime)
    {
        //Debug.Log("Reloading Gun...");
        isReloading = true;

        var reloadSound = weaponType.weaponSettings.ReloadSound;
        WeaponManager.Instance.AudioSource.PlayOneShot(reloadSound, 0.1f);

        yield return new WaitForSeconds(reloadTime);

        currentHoldingAmmo = maxClipAmmo;
        isEmpty = false;

        isReloading = false;
        //Debug.Log("Gun Reloaded!");
    }
}
