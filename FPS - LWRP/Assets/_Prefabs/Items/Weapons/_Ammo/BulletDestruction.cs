using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestruction : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5);
    }

    private void Update()
    {
        GetComponent<Rigidbody>().velocity += Vector3.up * Physics.gravity.y * (WeaponManager.Instance.bulletDropOff - 1) * Time.deltaTime; //bullet dropoff reapply into weapon manager
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        Debug.Log("Hit " + other.gameObject.name);

    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        Debug.Log("Hit " + collision.collider.gameObject.name);
    }
}
