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
    public readonly AffixType[] BaseAffixes;
    [DataMember]
    public readonly AffixType[] GuaranteedAffixes;
    [DataMember]
    public readonly AffixPool PossibleAffixes;

    public ItemBase(ItemBaseType baseType, ItemClass itemClass, AffixType[] baseAffixs, AffixType[] guaranteedAffixes, AffixPool affixPool)
    {
        BaseType = baseType;
        Class = itemClass;
        BaseAffixes = baseAffixs.Clone() as AffixType[];
        GuaranteedAffixes = guaranteedAffixes.Clone() as AffixType[];
        PossibleAffixes = affixPool;
    }

    public ItemBase(ItemBaseType baseType, ItemClass itemClass, AffixType[] baseAffixes, AffixType[] guaranteedAffixes, AffixPoolPreset affixPoolPreset)
        : this(baseType, itemClass, baseAffixes, guaranteedAffixes, AffixPool.GetPool(affixPoolPreset))
    { }

    /// <summary>
    /// Generates a new item of this base type
    /// </summary>
    /// <param name="tier">The tier of the item to be generated</param>
    /// <param name="quality">The quality of the item to be generated</param>
    /// <returns>The new item</returns>
    public abstract Item GenerateItem(int tier, int quality);

    /// <summary>
    /// Generates the Affixes and BaseAffixes for an item
    /// </summary>
    protected void GenerateAffixes(Item item)
    {
        item.Affixes.Clear();
        item.BaseAffixes.Clear();

        foreach (AffixType affixType in BaseAffixes)
        {
            item.BaseAffixes.Add(AffixInfo.GetAffixInfo(affixType).GenerateAffix(item.Tier));
        }

        // Create new lottery to decide tier roll of each affix
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

        // Guaranteed affixes don't care about possible pool
        foreach (AffixType affixType in GuaranteedAffixes)
        {
            int tier = tierLottery.Draw();
            tier = tier > quality ? quality : tier;
            item.Affixes.Add(AffixInfo.GetAffixInfo(affixType).GenerateAffix(tier));
            quality -= tier;
        }
        
        // Fill up the rest of the affixes
        Affix[] randomAffixes = PossibleAffixes.GetUniqueRandomAffixes(quality, tierLottery, new HashSet<AffixType>(GuaranteedAffixes));
        foreach (Affix affix in randomAffixes)
        {
            item.Affixes.Add(affix);
        }
    }
}

[DataContract]
public class WeaponBase : ItemBase
{
    [DataMember]
    public readonly AmmoClass[] AllowedAmmoTypes;

    public WeaponBase(ItemBaseType baseType, AffixType[] baseAffixes, AffixType[] guaranteedAffixes, AmmoClass[] allowedAmmoTypes, AffixPool affixPool)
        : base(baseType, ItemClass.Weapon, baseAffixes, guaranteedAffixes, affixPool)
    {
        AllowedAmmoTypes = allowedAmmoTypes.Clone() as AmmoClass[];
    }

    public WeaponBase(ItemBaseType baseType, AffixType[] baseAffixes, AffixType[] guaranteedAffixes, AmmoClass[] allowedAmmoTypes, AffixPoolPreset affixPoolPreset)
        : this(baseType, baseAffixes, guaranteedAffixes, allowedAmmoTypes, AffixPool.GetPool(affixPoolPreset))
    { }

    public override Item GenerateItem(int tier, int quality)
    {
        Weapon weapon = new Weapon(BaseType.ToString(), tier, quality, BaseType, this);
        GenerateAffixes(weapon);
        return weapon;
    }
}

[DataContract]
public class ArmorBase : ItemBase
{
    [DataMember]
    public readonly Slot[] Slots;

    public ArmorBase(ItemBaseType baseType, AffixType[] baseAffixes, AffixType[] guaranteedAffixes, Slot[] slots, AffixPool affixPool)
        : base(baseType, ItemClass.Armor, baseAffixes, guaranteedAffixes, affixPool)
    {
        Slots = slots.Clone() as Slot[];
    }

    public ArmorBase(ItemBaseType baseType, AffixType[] baseAffixes, AffixType[] guaranteedAffixes, Slot[] slots, AffixPoolPreset affixPoolPreset)
        : base(baseType, ItemClass.Armor, baseAffixes, guaranteedAffixes, AffixPool.GetPool(affixPoolPreset))
    { }

    public override Item GenerateItem(int tier, int quality)
    {
        Armor armor = new Armor(BaseType.ToString(), tier, quality, BaseType, this);
        GenerateAffixes(armor);
        return armor;
    }
}

public class AmmoBase : ItemBase
{
    [DataMember]
    public readonly AmmoClass AmmoClass;

    public AmmoBase(ItemBaseType baseType, AffixType[] baseAffixes, AffixType[] guaranteedAffixes, AmmoClass ammoClass, AffixPool affixPool)
        : base(baseType, ItemClass.Ammo, baseAffixes, guaranteedAffixes, affixPool)
    {
        AmmoClass = ammoClass;
    }

    public AmmoBase(ItemBaseType baseType, AffixType[] baseAffixes, AffixType[] guaranteedAffixes, AmmoClass ammoClass, AffixPoolPreset affixPoolPreset)
        : base(baseType, ItemClass.Ammo, baseAffixes, guaranteedAffixes, AffixPool.GetPool(affixPoolPreset))
    { }

    public override Item GenerateItem(int tier, int quality)
    {
        Ammo ammo = new Ammo(BaseType.ToString(), tier, quality, BaseType, this);
        GenerateAffixes(ammo);
        return ammo;
    }
}
