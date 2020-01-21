using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PlaceableItem", menuName = "Create Item/Item Type/Placeable", order = 2)]
public class Placeable : ItemType
{
    [Serializable]
    public struct PlaceableSettings
    {
        public GameObject ObjectToPlace;
        public GameObject PreviewObject;
    }

    public PlaceableSettings placeableSettings;
}
