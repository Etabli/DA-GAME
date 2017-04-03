using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a specific piece of Armor
/// </summary>
public class Armor : Item
{
    public Slot[] OccupiedSlots { get; protected set; }
}
