using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a single instance of an Affix.
/// </summary>
public class Affix
{
    public readonly AffixType Type;
    public readonly AffixValue Value;
    public readonly int Tier;

    // TODO: Decide if this is actually necessary
    protected readonly AffixInfo info;

    public Affix(AffixType type, AffixValue value, int tier)
    {
        Type = type;
        Value = value;
        Tier = tier;

        info = AffixInfo.GetAffixInfo(type);
    }

    public override string ToString()
    {
        return string.Format(info.Description, Value) + string.Format(" ({0})", Tier);
    }
}
