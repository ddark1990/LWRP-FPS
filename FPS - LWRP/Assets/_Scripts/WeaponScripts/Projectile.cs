using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPooledObject
{
    private int bulletDamage;
    private float damageMultiplier = 0f;
    private Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        BulletDropControl(WeaponManager.Instance.bulletDropOff);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DestroySelf();

        EffectsManager.Instance.PlayHitMarker(collision.collider, out damageMultiplier);
        WeaponImpactManager.PlayImpactEffect(collision.gameObject.tag, collision.GetContact(0).point, collision.GetContact(0).normal, EffectsManager.Instance.bulletImpactData);

        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("NPC")) && collision.gameObject.GetComponent<AiVitals>())
        {
            var aiVitals = collision.gameObject.GetComponent<AiVitals>();

            aiVitals.TakeDamage(bulletDamage * damageMultiplier); //fix bullet velocity application
        }
        //Debug.Log(collision.collider.name);

        if (collision.collider.gameObject.layer.Equals(LayerMask.NameToLayer("NPC")))
        {
            WeaponManager.Instance.ColliderHit = collision.collider;
            WeaponManager.Instance.BulletVelocity = rb.velocity.normalized;
        }
    }

    public void InitializeBullet(int _bulletDamage)
    {
        bulletDamage = _bulletDamage;
    }

    private void BulletDropControl(float dropOffFloat)
    {
        rb.velocity += Vector3.up * Physics.gravity.y * (dropOffFloat - 1) * Time.deltaTime;
    }

    private void DestroySelf()
    {
        SimplePoolManager.Instance.ReturnToPool(gameObject);
    }

    public void OnObjectSpawn(GameObject obj)
    {
        obj.GetComponentInChildren<TrailRenderer>().enabled = true;
    }

    public void OnObjectDespawn(GameObject obj) 
    {
        obj.GetComponentInChildren<TrailRenderer>().enabled = false;
        obj.GetComponent<Rigidbody>().Sleep();
    }
}
