using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

/// <summary>
/// An item base type
/// </summary>
public enum ItemBaseType
{
    //###------------------------------WEAPONS------------------------------###
    Pistol,
    Wand,

    //###-------------------------------ARMOR-------------------------------###
    Breastplate,
    Cockring,

    //###-------------------------------AMMO--------------------------------###
    PhysicalAmmo
}

[DataContract]
public class ItemBase
{
    [DataMember]
    public readonly ItemClass Class;
    [DataMember]
    public readonly AttributeType[] BaseAttributes;
    [DataMember]
    public readonly AttributeType[] GuaranteedAttributes;
    [DataMember]
    public readonly Slot[] Slots;

    public ItemBase(ItemClass itemClass, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes, Slot[] slots)
    {
        Class = itemClass;
        BaseAttributes = baseAttributes;
        GuaranteedAttributes = guaranteedAttributes;
        Slots = slots;
    }

    public Item GenerateItem(int tier, int quality)
    {
        throw new NotImplementedException();

        // TODO: Actually generate item
    }
}
