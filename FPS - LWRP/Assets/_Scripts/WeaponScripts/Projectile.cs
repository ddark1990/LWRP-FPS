using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Projectile : MonoBehaviour, IPooledObject
{
    private int _bulletDamage;
    private float _damageMultiplier = 0f;
    private Rigidbody _rb;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        EventRelay.OnBodyPartHit += BodyPartHit;
    }

    private void OnDestroy()
    {
        EventRelay.OnBodyPartHit -= BodyPartHit;
    }

    private void Update()
    {
        BulletDropControl(WeaponManager.Instance.bulletDropOff);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ReturnToPool();

        WeaponImpactManager.PlayImpactEffect(collision.gameObject.tag, collision.GetContact(0).point, collision.GetContact(0).normal, EffectsManager.Instance.bulletImpactData);
        EffectsManager.Instance.PlayHitMarker(collision.collider, out _damageMultiplier);

        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("NPC")) && collision.gameObject.GetComponent<AiVitals>())
        {
            var aiVitals = collision.gameObject.GetComponent<AiVitals>();

            aiVitals.TakeDamage(_bulletDamage * _damageMultiplier); //fix bullet velocity application
            
            WeaponManager.Instance.ColliderHit = collision.collider;
            WeaponManager.Instance.BulletVelocity = _rb.velocity.normalized;
            
            //hit body part event
            BodyPartHit(WeaponManager.Instance.ColliderHit);
        }
        else
        {
            //lose points by a set amount per miss
        }
    }

    private static string RemoveSpecialCharacters(string str) {
        var sb = new StringBuilder();
        foreach (var c in str) {
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_') {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
    
    public void BodyPartHit(Collider col)
    {
        //add points per body part
        var colliderNameSpecial = col.name.Replace("mixamorig", "");
        var colliderName = RemoveSpecialCharacters(colliderNameSpecial);
        
        PopupInfoController.Instance.AddPopUp("Hit " + colliderName);
    }
    
    public void InitializeBullet(int bulletDamage)
    {
        this._bulletDamage = bulletDamage;
    }

    private void BulletDropControl(float dropOff)
    {
        _rb.velocity += Vector3.up * (Physics.gravity.y * (dropOff - 1) * Time.deltaTime);
    }

    private void ReturnToPool()
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
