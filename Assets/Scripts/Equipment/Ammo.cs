using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoClass
{
    Bullet,
    Crystal,
    Battery
}

/// <summary>
/// Represents a specific stack of Ammo
/// </summary>
public class Ammo : Item
{
    static readonly int STACK_SIZE = 99;

    public AmmoClass AmmoClass
    {
        get
        {
            return ((AmmoBase)itemBase).AmmoClass;
        }
    }
    public int Count { get; protected set; }

    public Ammo(string name, int tier, int quality, ItemBaseType baseType, ItemBase itemBase) : base(name, tier, quality, baseType, itemBase)
    {
    }
}
