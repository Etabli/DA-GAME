using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The different affix types there are in the game.
/// </summary>
public enum AffixType
{
    None,
    Health,
    PhysDmgFlat,
    FireRate,
    Random
}

/// <summary>
/// Represents a single instance of an Affix.
/// </summary>
public class Affix
{ 
    public AffixType Type { get; protected set; }
    public AffixValueType Class { get; protected set; }
    public AffixValue Value { get; protected set; }
    public int Tier { get; protected set; }

    protected AffixInfo info;

    public Affix(AffixType type, AffixValueType affixClass, AffixValue value, int tier)
    {
        Type = type;
        Class = affixClass;
        Value = value;
        Tier = tier;

        info = AffixInfo.GetAffixInfo(type);
    }

    public override string ToString()
    {
        return string.Format(info.Description, Value) + string.Format(" ({0})", Tier);
    }
}
