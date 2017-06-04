using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a specific Weapon
/// </summary>
public class Weapon : Item
{
    protected List<Ammo> AmmoQueue;

    public Weapon(string name, int tier, int quality, ItemBaseType baseType, ItemBase itemBase) : base(name, tier, quality, baseType, itemBase)
    {
        AmmoQueue = new List<Ammo>();
    }
}
