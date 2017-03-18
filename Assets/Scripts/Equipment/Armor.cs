using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmorType
{
    Shirt,
    Cockring
}

/// <summary>
/// Represents a specific piece of Armor
/// </summary>
public class Armor : Item
{
    public ArmorType Type { get; protected set; }
    public Slot[] OccupiedSlots { get; protected set; }

    public Armor(ArmorType type)
    {
        Type = type;
        GenerateBaseAttributes();
    }

    public override void GenerateBaseAttributes()
    {

    }
}
