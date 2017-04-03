using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemClass
{
    Weapon,
    Armor,
    Ammo
}

/// <summary>
/// Class for basic item behavior
/// </summary>
public abstract class Item
{
    public string Name { get; protected set; }
    public int Tier { get; protected set; }
    public int Quality { get; protected set; }
    public ItemBaseType BaseType { get; protected set; }
    public Dictionary<AttributeType, Attribute> Attributes = new Dictionary<AttributeType, Attribute>();
    public Dictionary<AttributeType, Attribute> BaseAttributes = new Dictionary<AttributeType, Attribute>();

    protected ItemBase itemBase;

    public Item(string name, int tier, int quality, ItemBaseType baseType, ItemBase itemBase)
    {
        Name = name;
        Tier = tier;
        Quality = quality;
        BaseType = baseType;
        this.itemBase = itemBase;

        Attributes = new Dictionary<AttributeType, Attribute>();
        BaseAttributes = new Dictionary<AttributeType, Attribute>();
    }

    public override string ToString()
    {
        string result = string.Format("{0} (t{1}, q{2}): \n", Name, Tier, Quality);
        foreach(Attribute a in BaseAttributes.Values)
        {
            result += a + "\n";
        }
        result += "-------------------------------------------\n";
        
        foreach(Attribute a in Attributes.Values)
        {
            result += a + "\n";
        }
        result.TrimEnd('\n');
        
        return result;
    }
}
