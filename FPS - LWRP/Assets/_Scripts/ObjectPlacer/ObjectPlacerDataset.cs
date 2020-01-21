using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectPlacerDataset", menuName = "ObjectPlacer/Dataset")]
public class ObjectPlacerDataset : ScriptableObject
{
    public List<GameObject> objectsToPlace;
}
