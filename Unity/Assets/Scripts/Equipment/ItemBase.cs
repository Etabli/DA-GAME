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
    [DataMember]
    public readonly AttributePool PossibleAttributes;

    public ItemBase(ItemBaseType baseType, ItemClass itemClass, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes, AttributePool attributePool)
    {
        BaseType = baseType;
        Class = itemClass;
        BaseAttributes = baseAttributes.Clone() as AttributeType[];
        GuaranteedAttributes = guaranteedAttributes.Clone() as AttributeType[];
        PossibleAttributes = attributePool;
    }

    public ItemBase(ItemBaseType baseType, ItemClass itemClass, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes, AttributePoolPreset attributePoolPreset)
        : this(baseType, itemClass, baseAttributes, guaranteedAttributes, AttributePool.GetPool(attributePoolPreset))
    { }

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

        foreach (AttributeType attributeType in BaseAttributes)
        {
            item.BaseAttributes.Add(attributeType, AttributeInfo.GetAttributeInfo(attributeType).GenerateAttribute(item.Tier));
        }

        // Create new lottery to decide tier roll of each attribute
        // TODO: make nicer
        Lottery<int> tierLottery = new Lottery<int>();
        tierLottery.Enter(item.Tier, 10);
        if (item.Tier > 1)
            tierLottery.Enter(item.Tier - 1, 5);
        if (item.Tier > 2)
            tierLottery.Enter(item.Tier - 2, 2);
        if (item.Tier > 3)
            tierLottery.Enter(item.Tier - 3, 1);

        int quality = item.Quality;  

        // Guaranteed attributes don't care about possible pool
        foreach (AttributeType attributeType in GuaranteedAttributes)
        {
            int tier = tierLottery.GetWinner();
            tier = tier > quality ? quality : tier;
            item.Attributes.Add(attributeType, AttributeInfo.GetAttributeInfo(attributeType).GenerateAttribute(tier));
            quality -= tier;
        }
        
        // Fill up the rest of the attributes
        Attribute[] randomAttributes = PossibleAttributes.GetUniqueRandomAttributes(quality, tierLottery, new HashSet<AttributeType>(GuaranteedAttributes));
        foreach (Attribute attribute in randomAttributes)
        {
            item.Attributes.Add(attribute.Type, attribute);
        }
    }
}

[DataContract]
public class WeaponBase : ItemBase
{
    [DataMember]
    public readonly AmmoClass[] AllowedAmmoTypes;

    public WeaponBase(ItemBaseType baseType, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes, AmmoClass[] allowedAmmoTypes, AttributePool attributePool)
        : base(baseType, ItemClass.Weapon, baseAttributes, guaranteedAttributes, attributePool)
    {
        AllowedAmmoTypes = allowedAmmoTypes.Clone() as AmmoClass[];
    }

    public WeaponBase(ItemBaseType baseType, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes, AmmoClass[] allowedAmmoTypes, AttributePoolPreset attributePoolPreset)
        : this(baseType, baseAttributes, guaranteedAttributes, allowedAmmoTypes, AttributePool.GetPool(attributePoolPreset))
    { }

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

    public ArmorBase(ItemBaseType baseType, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes, Slot[] slots, AttributePool attributePool)
        : base(baseType, ItemClass.Armor, baseAttributes, guaranteedAttributes, attributePool)
    {
        Slots = slots.Clone() as Slot[];
    }

    public ArmorBase(ItemBaseType baseType, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes, Slot[] slots, AttributePoolPreset attributePoolPreset)
        : base(baseType, ItemClass.Armor, baseAttributes, guaranteedAttributes, AttributePool.GetPool(attributePoolPreset))
    { }

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

    public AmmoBase(ItemBaseType baseType, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes, AmmoClass ammoClass, AttributePool attributePool)
        : base(baseType, ItemClass.Ammo, baseAttributes, guaranteedAttributes, attributePool)
    {
        AmmoClass = ammoClass;
    }

    public AmmoBase(ItemBaseType baseType, AttributeType[] baseAttributes, AttributeType[] guaranteedAttributes, AmmoClass ammoClass, AttributePoolPreset attributePoolPreset)
        : base(baseType, ItemClass.Ammo, baseAttributes, guaranteedAttributes, AttributePool.GetPool(attributePoolPreset))
    { }

    public override Item GenerateItem(int tier, int quality)
    {
        Ammo ammo = new Ammo(BaseType.ToString(), tier, quality, BaseType, this);
        GenerateAttributes(ammo);
        return ammo;
    }
}
