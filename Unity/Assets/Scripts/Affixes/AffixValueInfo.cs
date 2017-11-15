﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    protected AffixValue baseValueMin;
    [DataMember]
    protected AffixValue baseValueMax;
    [DataMember]
    protected AffixProgression progression;

    public AffixValueInfo(AffixValue baseValueMin, AffixValue baseValueMax) : this(baseValueMin, baseValueMax, new AffixProgression("Linear", 0, 1))
    { }

    public AffixValueInfo(AffixValue baseValueMin, AffixValue baseValueMax, AffixProgression progression)
    {
        this.baseValueMin = baseValueMin;
        this.baseValueMax = baseValueMax;
        this.progression = progression;        
    }

    /// <summary>
    /// Generates an AffixValue of this type given a tier.
    /// </summary>
    /// <param name="tier">The tier of affix to be generated</param>
    /// <returns></returns>
    public AffixValue GetValueForTier(int tier)
    {
        return GetValueForTier(tier, progression);
    }

    public AffixValue GetValueForTier(int tier, AffixProgression prog)
    {
        if (!prog.HasProgressionFunction())
        {
            if (!prog.AttachProgressionFunction())
            {
                Debug.LogError(string.Format("Invalid progression function set for AffixValueInfo of type {0}", baseValueMin.GetType()));
            }
        }
        // At this point we are sure that the prog function is set, but not if parameters are valid

        float frac = UnityEngine.Random.Range(0, 100) / 100.0f;

        // Progress min and max values before combining them into a random value between them
        AffixValue progressedMin = prog.Apply(baseValueMin, tier);
        if (progressedMin == null)
        {
            // Parameters were invalid
            return null;
        }
        AffixValue progressedMax = prog.Apply(baseValueMax, tier);

        //Debug.Log(string.Format("Progressed minimum: {0}, Progressed maximum: {1}, frac: {2}", progressedMin, progressedMax, frac));

        return progressedMin + (progressedMax - progressedMin) * frac;
    }

    public override string ToString()
    {
        return string.Format("{0} - {1}", baseValueMin, baseValueMax);
    }
}