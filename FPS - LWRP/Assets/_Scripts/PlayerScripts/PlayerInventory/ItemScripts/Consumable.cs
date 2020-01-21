using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ConsumableItem", menuName = "Create Item/Item Type/Consumable", order = 0)]
public class Consumable : ItemType
{
    [Serializable]
    public struct ConsumableSettings
    {
        public bool Cookable;
        public bool Raw;
        public float GiveHealthAmount;
        public float GiveCaloriesAmount;
        public float GiveHydrationAmount;
    }

    public ConsumableSettings consumableSettings;
}
