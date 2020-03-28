using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerDebug : MonoBehaviour
{
    public Transform SpawnLocation;
    public GameObject SpawnPrefab;

    private void Start()
    {
        //InvokeRepeating("SpawnPumpkin", 1, 1);
    }

    public void Spawn()
    {
        var prefab = SimplePoolManager.Instance.SpawnFromPool(SpawnPrefab.name, SpawnLocation.position, SpawnLocation.rotation);

        //prefab.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * 10000, ForceMode.Acceleration);
    }
}
