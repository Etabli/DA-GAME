using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The different attribute types there are in the game.
/// </summary>
public enum AttributeType
{
    Health,
    PhysDmgFlat,
    FireRate
}

/// <summary>
/// Represents a single instance of an Attribute.
/// </summary>
public class Attribute
{ 
    public AttributeType Type { get; protected set; }
    public AttributeValueType Class { get; protected set; }
    public AttributeValue Value { get; protected set; }

    public Attribute(AttributeType type, AttributeValueType attributeClass, AttributeValue value)
    {
        Type = type;
        Class = attributeClass;
        Value = value;
    }

    public override string ToString()
    {
        return string.Format("{0}({1}): {2}", Type, Class, Value);
    }
}
