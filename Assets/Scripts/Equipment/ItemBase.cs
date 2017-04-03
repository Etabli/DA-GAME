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
    PhysicalBullets,
    FireCrystals
}

[DataContract]
public abstract class ItemBase
{
    [DataMember]
    public readonly ItemClass Class;
    [DataMember]
    public readonly ItemBaseType BaseType;
    [DataMember]
    public readonly AttributeType[] BaseAttributes;
    [DataMember]
    public readonly AttributeType[] GuaranteedAttributes;

    public ItemBase(ItemClass itemClass, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes)
    {
        Class = itemClass;
        BaseAttributes = baseAttributes.Clone() as AttributeType[];
        GuaranteedAttributes = guaranteedAttributes.Clone() as AttributeType[];
    }

    /// <summary>
    /// Generates a new item of this base type
    /// </summary>
    /// <param name="tier">The tier of the item to be generated</param>
    /// <param name="quality">The quality of the item to be generated</param>
    /// <returns>The new item</returns>
    public abstract Item GenerateItem(int tier, int quality);

    /// <summary>
    /// Generates the Attributes and BaseAttributes for an item
    /// </summary>
    protected void GenerateAttributes(Item item)
    {
        item.Attributes.Clear();
        item.BaseAttributes.Clear();
    }
}

[DataContract]
public class WeaponBase : ItemBase
{
    [DataMember]
    public readonly AmmoClass[] AllowedAmmoTypes;

    public WeaponBase(ItemClass itemClass, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes, AmmoClass[] allowedAmmoTypes) : base(itemClass, baseAttributes, guaranteedAttributes)
    {
        AllowedAmmoTypes = allowedAmmoTypes.Clone() as AmmoClass[];
    }

    public override Item GenerateItem(int tier, int quality)
    {
        Weapon weapon = new Weapon(BaseType.ToString(), tier, quality, BaseType, this);
        GenerateAttributes(weapon);
        return weapon;
    }
}

[DataContract]
public class ArmorBase : ItemBase
{
    [DataMember]
    public readonly Slot[] Slots;

    public ArmorBase(ItemClass itemClass, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes, Slot[] slots) : base(itemClass, baseAttributes, guaranteedAttributes)
    {
        Slots = slots.Clone() as Slot[];
    }

    public override Item GenerateItem(int tier, int quality)
    {
        Armor armor = new Armor(BaseType.ToString(), tier, quality, BaseType, this);
        GenerateAttributes(armor);
        return armor;
    }
}

public class AmmoBase : ItemBase
{
    [DataMember]
    public readonly AmmoClass AmmoClass;

    public AmmoBase(ItemClass itemClass, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes, AmmoClass ammoClass) : base(itemClass, baseAttributes, guaranteedAttributes)
    {
        AmmoClass = ammoClass;
    }

    public override Item GenerateItem(int tier, int quality)
    {
        Ammo ammo = new Ammo(BaseType.ToString(), tier, quality, BaseType, this);
        GenerateAttributes(ammo);
        return ammo;
    }
}
