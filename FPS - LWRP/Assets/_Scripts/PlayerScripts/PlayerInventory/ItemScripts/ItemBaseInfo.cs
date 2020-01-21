using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBaseInfo : ScriptableObject
{
    public enum ItemType { Consumable, Weapon, Armor, Placeable, Resource }
    [Header("Item Type")]
    public ItemType itemType;

    [Header("Item Information")]
    public string ItemName;
    [TextArea] public string ItemDescription;
    public Sprite ItemSprite;
    public int StackLimit;

    [Header("Interact Sounds")]
    public AudioClip UseItemSound;

    [Header("Item Pickup/Drop Sounds")]
    public AudioClip PickUpItemSound;
    public AudioClip DropItemSound;

    [Header("Item UI Sounds")]
    public AudioClip[] SelectSounds; 
    public AudioClip DragSound; 
    public AudioClip DropSound; 

    /*
    [Header("Durability Options")]
    public bool HasDurability;
    public float DurabilityAmmount;
    */
}
