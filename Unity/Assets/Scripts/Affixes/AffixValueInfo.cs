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
    { }

    public AffixValueInfo(AffixValue baseValueMin, AffixValue baseValueMax) : this(baseValueMin, baseValueMax, new AffixProgression("Linear", 0, 1))
    { }

    public AffixValueInfo(AffixValue baseValueMin, AffixValue baseValueMax, AffixProgression progression)
    {
        BaseValueMin = baseValueMin;
        BaseValueMax = baseValueMax;
        Progression = new AffixProgression(progression);
    }

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
            if (!prog.AttachProgressionFunction())
            {
                //Debug.LogError(string.Format("Invalid progression function set for AffixValueInfo of type {0}", baseValueMin.GetType()));
                throw new InvalidOperationException("Trying to generate affix value with no or invalid progression function set!");
            }
        }
        // At this point we are sure that the prog function is set, but not if parameters are valid

        var rng = new System.Random();
        float frac = rng.Next(101) / 100.0f;

        // Progress min and max values before combining them into a random value between them
        AffixValue progressedMin = prog.Apply(BaseValueMin, tier);
        if (progressedMin == null)
        {
            // Parameters were invalid
            return null;
        }
        AffixValue progressedMax = prog.Apply(BaseValueMax, tier);

        //Debug.Log(string.Format("Progressed minimum: {0}, Progressed maximum: {1}, frac: {2}", progressedMin, progressedMax, frac));

        return progressedMin + (progressedMax - progressedMin) * frac;
    }

    public override string ToString()
    {
        return string.Format("{0} - {1}", BaseValueMin, BaseValueMax);
    }
}
