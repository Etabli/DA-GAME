using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AmmoType
{
    Bullet,
    Crystal
}

/// <summary>
/// Represents a specific stack of Ammo
/// </summary>
public class Ammo : Item
{
    static readonly int STACK_SIZE = 99;

    public AmmoType Type { get; protected set; }
    public int Count { get; protected set; }

    public Ammo(AmmoType type)
    {
        Type = type;
    }
}
