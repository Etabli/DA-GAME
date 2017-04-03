using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a specific Weapon
/// </summary>
public class Weapon : Item
{
    public AmmoType[] AllowedAmmoTypes { get; protected set; }

    protected List<Ammo> AmmoQueue;
}
