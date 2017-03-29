using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Pistol,
    Fireball
}

/// <summary>
/// Represents a specific Weapon
/// </summary>
public class Weapon : Item
{
    public WeaponType Type { get; protected set; }
    public AmmoType[] AllowedAmmoTypes { get; protected set; }

    protected List<Ammo> AmmoQueue;

    public Weapon(WeaponType type)
    {
        Type = type;
    }
}
