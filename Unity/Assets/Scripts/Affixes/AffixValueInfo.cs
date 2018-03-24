using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

/// <summary>
/// Represents information on an Affix's value or values. Serialiazable via a DataContract.
/// </summary>
[DataContract]
[KnownType(typeof(AffixValueSingle))]
[KnownType(typeof(AffixValueRange))]
[KnownType(typeof(AffixValueMultiple))]
public class AffixValueInfo
{
    [DataMember]
    public readonly AffixValue BaseValueMin;
    [DataMember]
    public readonly AffixValue BaseValueMax;
    [DataMember]
    public readonly AffixProgression Progression;

    public AffixValueInfo(AffixValueInfo src) : this(src.BaseValueMin, src.BaseValueMax, src.Progression)
    {}

    public AffixValueInfo() : this(new AffixValueSingle(), new AffixValueSingle())
    { }

    public AffixValueInfo(AffixValue baseValueMin, AffixValue baseValueMax) : this(baseValueMin, baseValueMax, new AffixProgression("Linear", 0, 1))
    { }

    public AffixValueInfo(AffixValue baseValueMin, AffixValue baseValueMax, AffixProgression progression)
    {
        if (!baseValueMin.IsSameType(baseValueMax))
            throw new ArgumentException($"Mininum and maximum value have to be of the same type!");

        BaseValueMin = baseValueMin;
        BaseValueMax = baseValueMax;
        Progression = new AffixProgression(progression);
    }

    #region Comparison
    public override bool Equals(object obj)
    {
        if (obj is AffixValueInfo)
            return this == (AffixValueInfo)obj;
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator==(AffixValueInfo lhs, AffixValueInfo rhs)
    {
        if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
            return true;
        if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            return false;

        if (lhs.BaseValueMin != rhs.BaseValueMin ||
            lhs.BaseValueMax != rhs.BaseValueMax ||
            lhs.Progression != rhs.Progression)
        {
            return false;
        }
        return true;
    }

    public static bool operator!=(AffixValueInfo lhs, AffixValueInfo rhs)
    {
        return !(lhs == rhs);
    }
    #endregion

    /// <summary>
    /// Generates an AffixValue of this type given a tier.
    /// </summary>
    /// <param name="tier">The tier of affix to be generated</param>
    /// <returns></returns>
    public AffixValue GetValueForTier(int tier)
    {
        return GetValueForTier(tier, Progression);
    }

    public AffixValue GetValueForTier(int tier, AffixProgression prog)
    {
        if (!prog.HasProgressionFunction())
        {
            throw new InvalidOperationException("Trying to generate affix value with no or invalid progression function set!");
        }
        // At this point we are sure that the prog function is set, but not if parameters are valid

        var rng = new Random();
        float frac = rng.Next(101) / 100.0f;

        // This will raise an exception if parameters are invalid
        AffixValue progressedMin = prog.Apply(BaseValueMin, tier);
        AffixValue progressedMax = prog.Apply(BaseValueMax, tier);

        return progressedMin + (progressedMax - progressedMin) * frac;
    }

    public override string ToString()
    {
        return string.Format("{0} - {1}", BaseValueMin, BaseValueMax);
    }
}
