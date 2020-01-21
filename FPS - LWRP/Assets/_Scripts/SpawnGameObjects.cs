using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGameObjects : MonoBehaviour
{
    [Tooltip("density amount x obj arr length")] [Range(0, 1000)] public int objectDensity;
    public Vector3 gridSize;
    public ObjectPlacerDataset objectPlacerDataset;
    public bool randomizeHeight;
    public float minHeight = 1f;
    public float maxHeight = 1f;
    //public GameObject[] objectsToSpawn;

    public List<GameObject> tempObjList;


    public void PlaceRandomObjects()
    {
        for (int i = 0; i < objectDensity; i++)
        {
            for (int z = 0; z < objectPlacerDataset.objectsToPlace.Count; z++)
            {
                var rndIndex = Random.Range(0, objectPlacerDataset.objectsToPlace.Count);
                var randomObj = objectPlacerDataset.objectsToPlace[rndIndex];

                var randomPos = transform.position + new Vector3(Random.Range(-gridSize.x, gridSize.x), 0, Random.Range(-gridSize.z, gridSize.z)) / 2;
                var randomRot = new Vector3(0, Random.Range(0, 360), 0);

                var spawnedObj = Instantiate(randomObj, transform);

                spawnedObj.transform.position = randomPos;
                spawnedObj.transform.rotation = Quaternion.Euler(randomRot);

                if(randomizeHeight)
                {
                    spawnedObj.transform.localScale = new Vector3(1, Random.Range(minHeight, maxHeight), 1);
                }

                tempObjList.Add(spawnedObj);
            }
        }

        Debug.Log("Placed " + tempObjList.Count + " objects.");
    }

    public void RemovePlacedObjects()
    {
        Debug.Log("Cleared " + tempObjList.Count + " objects.");

        for (int i = 0; i < tempObjList.Count; i++)
        {
            DestroyImmediate(tempObjList[i]);
        }

        tempObjList.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, gridSize);
    }
}
