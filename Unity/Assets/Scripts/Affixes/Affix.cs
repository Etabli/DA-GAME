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
    public readonly AffixType Type;
    public readonly AffixValueType Class;
    public readonly AffixValue Value;
    public readonly int Tier;

    protected readonly AffixInfo info;

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
