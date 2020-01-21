using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrails : MonoBehaviour
{
    public bool EnableBulletTrails;
    public Vector3 boobs;

    private WeaponManager weaponManager;
    private LineRenderer lineRenderer;


    void Start()
    {
        weaponManager = WeaponManager.Instance;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!EnableBulletTrails || !weaponManager.ActiveRig)
        {
            lineRenderer.enabled = false;
            return;
        }
        boobs = GetStartingRayPosition();

        DrawRay();
    }

    private Vector3 GetStartingRayPosition()
    {
        return Input.GetKeyDown(KeyCode.Mouse0) ? transform.TransformDirection(weaponManager.ActiveRig.GetComponent<RigData>().ShootPoint.position) : Vector3.zero;
    }

    private void DrawRay()
    {
        if(!lineRenderer.enabled) lineRenderer.enabled = true;

        var tempShootPoint = GetStartingRayPosition();
        var shotBullet = weaponManager.bulletInstance;

        lineRenderer.SetPosition(0, tempShootPoint);
        lineRenderer.SetPosition(1, shotBullet.transform.position);
        Debug.Log("DrawingRAys!");

    }
}
