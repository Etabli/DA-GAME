using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a specific stack of Ammo
/// </summary>
public class Ammo : Item
{
    static readonly int STACK_SIZE = 99;
    
    public int Count { get; protected set; }
}
