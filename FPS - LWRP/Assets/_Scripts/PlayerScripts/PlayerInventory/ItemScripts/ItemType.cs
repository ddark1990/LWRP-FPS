using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemType : ScriptableObject
{
    [Serializable]
    public struct ItemData
    {
        public string ItemName;
        [TextArea] public string ItemDescription;
        public Sprite ItemSprite;
        public int StackLimit;
    }
    [Serializable]
    public struct ItemSoundData
    {
        [Header("Interact Sounds")]
        public AudioClip ActivateItemSound;

        [Header("Item Pickup/Drop Sounds")]
        public AudioClip PickUpItemSound;
        public AudioClip ThrowItemSound;

        [Header("Item UI Sounds")]
        public AudioClip[] SelectSounds;
        public AudioClip DragSound;
        public AudioClip DropSound;
    }

    public ItemData itemData;
    public ItemSoundData itemSoundData;
}
