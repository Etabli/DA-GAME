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
    private readonly AffixValue baseValueMin;
    [DataMember]
    private readonly AffixValue baseValueMax;
    [DataMember]
    private readonly AffixProgression[] progressions;

    public AffixValue BaseValueMin { get { return baseValueMin; } }
    public AffixValue BaseValueMax { get { return baseValueMax; } }
    public AffixProgression Progression { get { return progressions[0]; } }
    public AffixProgression[] Progressions { get { return progressions; } }

    public AffixValueInfo(AffixValueInfo src) : this(src.baseValueMin, src.baseValueMax, src.progressions)
    {
    }

    public AffixValueInfo() : this(new AffixValueSingle(), new AffixValueSingle())
    { }

    public AffixValueInfo(AffixValue baseValueMin, AffixValue baseValueMax) : this(baseValueMin, baseValueMax, new AffixProgression("Linear", 0, 1))
    { }

    public AffixValueInfo(AffixValue baseValueMin, AffixValue baseValueMax, AffixProgression progression)
        : this (baseValueMin, baseValueMax, new AffixProgression[] { progression })
    { }

    public AffixValueInfo(AffixValue baseValueMin, AffixValue baseValueMax, AffixProgression[] progressions)
    {
        if (!baseValueMin.IsSameType(baseValueMax))
            throw new ArgumentException($"Mininum and maximum value have to be of the same type!");

        if (BaseValueMin is AffixValueMultiple)
            if ((BaseValueMin as AffixValueMultiple).Count != progressions.Length)
                throw new ArgumentException($"Info AffixValueMultiple needs the same amount of progressions as affix values!", nameof(progressions));

        this.baseValueMin = baseValueMin;
        this.baseValueMax = baseValueMax;
        this.progressions = new AffixProgression[progressions.Length];
        for (int i = 0; i < progressions.Length; i++)
        {
            this.progressions[i] = new AffixProgression(progressions[i]);
        }
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

        if (lhs.baseValueMin != rhs.baseValueMin ||
            lhs.baseValueMax != rhs.baseValueMax ||
            lhs.progressions != rhs.progressions)
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
        if (baseValueMin is AffixValueMultiple)
        {
            List<AffixValue> values = new List<AffixValue>();

            var min = baseValueMin as AffixValueMultiple;
            var max = baseValueMax as AffixValueMultiple;

            for (int i = 0; i < min.Count; i++)
            {
                values.Add(GetValueForTier(min[i], max[i], tier, progressions[i]));
            }

            return new AffixValueMultiple(values.ToArray());
        }
        else
            return GetValueForTier(baseValueMin, baseValueMax, tier, Progression);
    }

    /// <summary>
    /// Generates an affixValue based on min and max values, a tier, and a progression function
    /// </summary>
    protected AffixValue GetValueForTier(AffixValue min, AffixValue max, int tier, AffixProgression prog)
    {
        if (!prog.HasProgressionFunction())
        {
            throw new InvalidOperationException("Trying to generate affix value with no or invalid progression function set!");
        }
        // At this point we are sure that the prog function is set, but not if parameters are valid

        var rng = new Random();
        float frac = rng.Next(101) / 100.0f;

        // This will raise an exception if parameters are invalid
        AffixValue progressedMin = prog.Apply(min, tier);
        AffixValue progressedMax = prog.Apply(max, tier);

        return progressedMin + (progressedMax - progressedMin) * frac;
    }

    public override string ToString()
    {
        return string.Format("{0} - {1}", baseValueMin, baseValueMax);
    }
}
