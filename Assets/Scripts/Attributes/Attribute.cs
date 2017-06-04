using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The different attribute types there are in the game.
/// </summary>
public enum AttributeType
{
    None,
    Health,
    PhysDmgFlat,
    FireRate,
    Random
}

/// <summary>
/// Represents a single instance of an Attribute.
/// </summary>
public class Attribute
{ 
    public AttributeType Type { get; protected set; }
    public AttributeValueType Class { get; protected set; }
    public AttributeValue Value { get; protected set; }
    public int Tier { get; protected set; }

    protected AttributeInfo info;

    public Attribute(AttributeType type, AttributeValueType attributeClass, AttributeValue value, int tier)
    {
        Type = type;
        Class = attributeClass;
        Value = value;
        Tier = tier;

        info = AttributeInfo.GetAttributeInfo(type);
    }

    public override string ToString()
    {
        return string.Format(info.Description, Value) + string.Format(" ({0})", Tier);
    }
}
