using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Hands,
    NotHands,
    Feet
}

/// <summary>
/// Abstract class for basic item behavior
/// </summary>
public abstract class Item
{
    public string Name { get; protected set; }
    public int Tier { get; protected set; }
    public int Quality { get; protected set; }
    public Dictionary<AttributeType, Attribute> Attributes = new Dictionary<AttributeType, Attribute>();
    public Dictionary<AttributeType, Attribute> BaseAttributes = new Dictionary<AttributeType, Attribute>();

    public abstract void GenerateBaseAttributes();
    public void GenerateAttributes(int Quality)
    {

    }
}
