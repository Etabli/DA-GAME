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
    public Dictionary<AffixType, Affix> Affixes = new Dictionary<AffixType, Affix>();
    public Dictionary<AffixType, Affix> BaseAffixes = new Dictionary<AffixType, Affix>();

    protected ItemBase itemBase;

    public Item(string name, int tier, int quality, ItemBaseType baseType, ItemBase itemBase)
    {
        Name = name;
        Tier = tier;
        Quality = quality;
        BaseType = baseType;
        this.itemBase = itemBase;

        Affixes = new Dictionary<AffixType, Affix>();
        BaseAffixes = new Dictionary<AffixType, Affix>();
    }

    public override string ToString()
    {
        string result = string.Format("{0} (t{1}, q{2}): \n", Name, Tier, Quality);
        foreach (Affix a in BaseAffixes.Values)
        {
            result += a + "\n";
        }
        result += "-------------------------------------------\n";

        foreach (Affix a in Affixes.Values)
        {
            result += a + "\n";
        }
        result.TrimEnd('\n');

        return result;
    }
}
