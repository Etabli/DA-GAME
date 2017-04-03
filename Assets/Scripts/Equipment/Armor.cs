using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a specific piece of Armor
/// </summary>
public class Armor : Item
{
    public Armor(string name, int tier, int quality, ItemBaseType baseType, ItemBase itemBase) : base(name, tier, quality, baseType, itemBase)
    {
    }
}
