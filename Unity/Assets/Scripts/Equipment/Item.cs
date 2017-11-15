using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum  ItemClass
{
    Armor,
    Weapon,
    Ammo
}

/// <summary>
/// Abstract class for basic item behavior
/// </summary>
public abstract class Item
{
    public string Name { get; protected set; }
    public int Tier { get; protected set; }
    public int Quality { get; protected set; }
    public ItemBaseType BaseType { get; protected set; }
    public AffixContainer Affixes = new AffixContainer();
    public AffixContainer BaseAffixes = new AffixContainer();

    protected ItemBase itemBase;

    public Item(string name, int tier, int quality, ItemBaseType baseType, ItemBase itemBase)
    {
        Name = name;
        Tier = tier;
        Quality = quality;
        BaseType = baseType;
        this.itemBase = itemBase;
    }

    public override string ToString()
    {
        string result = string.Format("{0} (t{1}, q{2}): \n", Name, Tier, Quality);
        result += BaseAffixes.ToString();
        result += "\n------------------------------------\n";
        result += Affixes.ToString();
        return result;
    }
}
