using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GoomerFPSController;
using Photon.Pun;
using UnityEngine.Scripting;

public class WeaponManager : MonoBehaviourPunCallbacks
{
    public static WeaponManager Instance;

    public Transform ActiveRig;
    public Item EquipedWeapon;

    public Transform RigHolder;

    [HideInInspector] public bool lookingDownSights;
    public bool WeaponEquiped;
    [SerializeField] private float lookSpeed = 1f;
    [SerializeField] private float zoomFovAmount = 5f;

    [Header("Cache")]
    public WeaponSway WeaponSway;
    public AudioSource AudioSource;

    [Header("TestingVar")]
    public float forceMultiplier = 1000;

    public float bulletDropOff { get; private set; }
    public GameObject bulletInstance { get; private set; }
    public Collider ColliderHit;
    public Vector3 BulletVelocity;

    private Vector3 OrigGunPos;
    private Vector3 OrigGunRot;

    private GameObject _activeRig;
    private float startFov;
    private float timeToFire = 0f;
    private bool resetToOrigPos;
    private int currentGunAmmo;
    private GunReloadController gunReloadController;
    private Coroutine reloadCoroutine;
    private Item tempWep;

    private bool isAutomatic;
    private GameObject bullet;
    private GameObject shell;
    private AudioClip[] shootSounds;
    private float recoilUpKick;
    private float recoilSideKick;
    private float fireRate;
    private float bulletDrop;
    private int bulletDamage;
    //var forceMultiplier = equipedWeapon.ItemTypeInfo.GetType().GetField("ForceMultiplier").GetValue(equipedWeapon.ItemTypeInfo);

    private Transform shootPoint;
    private Transform shellEjectPoint;
    private Vector3 facingDir;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        startFov = 50;
    }
    private void FixedUpdate()
    {
        if (!ActiveRig) return;

        LookDownSights(ActiveRig);
    }
    private void Update()
    {
        ReloadGun(EquipedWeapon);

        if (!ActiveRig || FPSController.Instance.MidAir) return;

        ShootGun(ActiveRig, EquipedWeapon);
    }

    private void ShootGun(Transform activeRig, Item equipedWeapon)
    {
        GarbageCollector.CollectIncremental((ulong)0.1f);

        gunReloadController = equipedWeapon.GetComponent<GunReloadController>();

        if (InventoryManager.Instance.MenuOpen || gunReloadController.isReloading) return;

        SetGunData(activeRig, equipedWeapon);

        //recoilUpKick = Random.Range(1, (float)recoilUpKick);
        recoilSideKick = Random.Range(-recoilSideKick, recoilSideKick);

        bulletDropOff = bulletDrop; //sets data for projectiles dropoff method

        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= timeToFire && isAutomatic && gunReloadController.currentHoldingAmmo > 0)
        {
            timeToFire = Time.time + 1f / fireRate;

            bulletInstance = ShootBullet(equipedWeapon, bullet.name, shell, shellEjectPoint, shootPoint, facingDir, forceMultiplier, shootSounds, recoilUpKick, recoilSideKick);
            bulletInstance.GetComponent<Projectile>().InitializeBullet(bulletDamage);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= timeToFire && gunReloadController.currentHoldingAmmo > 0)
        {
            timeToFire = Time.time + 1f / fireRate;

            bulletInstance = ShootBullet(equipedWeapon, bullet.name, shell, shellEjectPoint, shootPoint, facingDir, forceMultiplier, shootSounds, recoilUpKick, recoilSideKick);
            bulletInstance.GetComponent<Projectile>().InitializeBullet(bulletDamage);
        }
    }
    private GameObject ShootBullet(Item equipedWeapon, string bulletName, GameObject shellPrefab, Transform shellEjectPoint, Transform shootPoint, Vector3 force, float forceMultiplier, AudioClip[] audioClips, float recoilUpKick, float recoilSideKick)
    {
        var rigData = ActiveRig.GetComponent<RigData>();

        var bullet = SimplePoolManager.Instance.SpawnFromPool(bulletName, shootPoint.position, shootPoint.rotation); //spawn bullet
        bullet.GetComponent<Rigidbody>().AddForce(force * forceMultiplier, ForceMode.VelocityChange); //send it

        var rndNum = Random.Range(0, audioClips.Length); //shoot sound
        AudioSource.PlayOneShot(audioClips[rndNum], .1f);

        rigData.MuzzleFlash.Play(); //muzzle effect

        WeaponRecoil.Instance.ApplyGunRecoil(recoilUpKick, recoilSideKick);

        if (shellPrefab) //move into shelleject method
        {
            var randomRot = Random.Range(-360, 360);

            var shell = Instantiate(shellPrefab, shellEjectPoint.position, Quaternion.Euler(randomRot, randomRot, randomRot));
            shell.GetComponent<Rigidbody>().AddForce(PlayerSelection.Instance.Cam.transform.right * 1, ForceMode.Impulse);
        }

        if (gunReloadController.maxClipAmmo >= 0) //move into decrement/update bullet count
        {
            gunReloadController.currentHoldingAmmo--;

            if (gunReloadController.currentHoldingAmmo == 0)
                gunReloadController.isEmpty = true;
        }

        return bullet;
    }

    private void SetGunData(Transform activeRig, Item equipedWeapon)
    {
        var weaponType = equipedWeapon.ItemType as Weapon;

        isAutomatic = weaponType.weaponSettings.Automatic;
        bullet = weaponType.weaponSettings.BulletPrefab;
        shell = weaponType.weaponSettings.ShellPrefab;
        shootSounds = weaponType.weaponSettings.ShootSounds;
        recoilUpKick = weaponType.weaponSettings.RecoilUpKick;
        recoilSideKick = weaponType.weaponSettings.RecoilSideKick;
        fireRate = weaponType.weaponSettings.FireRate;
        bulletDrop = weaponType.weaponSettings.BulletDrop;
        bulletDamage = weaponType.weaponSettings.BulletDamage;
        //var forceMultiplier = equipedWeapon.ItemTypeInfo.GetType().GetField("ForceMultiplier").GetValue(equipedWeapon.ItemTypeInfo);

        shootPoint = activeRig.GetComponent<RigData>().ShootPoint;
        shellEjectPoint = activeRig.GetComponent<RigData>().ShellDischargePoint;
        facingDir = transform.TransformDirection(Vector3.forward);
    }
    public void ToggleActiveWeapon(Item weaponToEquip)
    {
        if (PhotonNetwork.IsConnected && !transform.root.GetComponent<PhotonView>().IsMine) return;

        WeaponEquiped = !WeaponEquiped;
        EquipedWeapon = weaponToEquip;

        var weaponType = weaponToEquip.ItemType as Weapon;

        var weaponRig = weaponType.weaponSettings.RigPrefab;

        if (WeaponEquiped) //activate weapon rig animation
        {
            _activeRig = Instantiate(weaponRig, RigHolder);
            ActiveRig = _activeRig.transform;
        }
        else //reset the active rig
        {
            ResetRig();

            WeaponSway.ResetSwayTransforms();
        }

        WeaponSway.enabled = WeaponEquiped;
    }
    public void ResetRig()
    {
        ActiveRig = null;
        Destroy(_activeRig);
        _activeRig = null;
        EquipedWeapon = null;
        WeaponEquiped = false;
        WeaponSway.enabled = false;
        InventoryManager.Instance.UseSlot = null;
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void LookDownSights(Transform activeRig)
    {
        float animSpeed = Time.deltaTime * lookSpeed;

        if (Input.GetKey(KeyCode.Mouse1) && !FPSController.Instance.MidAir)
        {
            //if (Input.GetKeyDown(KeyCode.Mouse1))
            //{
            //    resetToIdlePos = true;

            //    if (resetToIdlePos)
            //    {
            //        activeRig.localPosition = Vector3.Slerp(activeRig.localPosition, OrigGunPos, animSpeed * 3);
            //        activeRig.localRotation = Quaternion.Slerp(activeRig.localRotation, Quaternion.Euler(OrigGunRot), animSpeed * 3);
            //    }
            //}

            activeRig.localPosition = Vector3.Slerp(activeRig.localPosition, activeRig.GetComponent<RigData>().LookDownSightsRef.localPosition, animSpeed);
            activeRig.localRotation = Quaternion.Slerp(activeRig.localRotation, Quaternion.Euler(activeRig.GetComponent<RigData>().LookDownSightsRef.localRotation.eulerAngles), animSpeed);

            var v = startFov - zoomFovAmount;
            PlayerSelection.Instance.Cam.fieldOfView = Mathf.Lerp(PlayerSelection.Instance.Cam.fieldOfView, v, animSpeed);

            var sens = FPSController.Instance.MouseSensetivity / 1.5f;
            //GoomerFPSController.FPSController.Instance.MouseSensetivity = sens;

            lookingDownSights = true;
        }
        else if (activeRig.localPosition != activeRig.GetComponent<RigData>().OrigPosRef.localPosition || activeRig.localRotation != Quaternion.Euler(activeRig.GetComponent<RigData>().OrigPosRef.localRotation.eulerAngles))
        {
            activeRig.localPosition = Vector3.Slerp(activeRig.localPosition, activeRig.GetComponent<RigData>().OrigPosRef.localPosition, animSpeed);
            activeRig.localRotation = Quaternion.Slerp(activeRig.localRotation, Quaternion.Euler(activeRig.GetComponent<RigData>().OrigPosRef.localRotation.eulerAngles), animSpeed);

            //Debug.Log(activeRig.GetComponent<RigData>().OrigPosRef);

            var v = startFov;
            PlayerSelection.Instance.Cam.fieldOfView = Mathf.Lerp(PlayerSelection.Instance.Cam.fieldOfView, v, animSpeed);

            lookingDownSights = false;
            resetToOrigPos = false;
        }
    }

    private void ReloadGun(Item equipedWeapon)
    {
        if (reloadCoroutine != null && tempWep != equipedWeapon)
        {
            Debug.Log("CanceledReload");
            AudioSource.Stop();
            gunReloadController.isReloading = false;
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            return;
        }

        if (!equipedWeapon) return;

        var weaponType = equipedWeapon.ItemType as Weapon;
        var reloadTime = weaponType.weaponSettings.ReloadTime;

        if (Input.GetKeyDown(InputManager.Instance.InputKeyManager.ReloadKey) && !gunReloadController.isReloading && !gunReloadController.currentHoldingAmmo.Equals(gunReloadController.maxClipAmmo))
        {
            reloadCoroutine = StartCoroutine(gunReloadController.Reload(reloadTime));

            tempWep = equipedWeapon;
        }
    }

}
