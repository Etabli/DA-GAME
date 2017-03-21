using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

public delegate AttributeValue Progression(int tier, AttributeValue baseValue);

/// <summary>
/// Represents information on an Attribute's value or values. Serialiazable via a DataContract.
/// </summary>
[DataContract]
[KnownType(typeof(AttributeValueSingle))]
[KnownType(typeof(AttributeValueRange))]
[KnownType(typeof(AttributeValueMultiple))]
public class AttributeValueInfo
{
    [DataMember]
    protected AttributeValue baseValueMin;
    [DataMember]
    protected AttributeValue baseValueMax;

    protected Progression progression;

    public AttributeValueInfo(AttributeValue baseValueMin, AttributeValue baseValueMax) : this(baseValueMin, baseValueMax, null)
    { }

    public AttributeValueInfo(AttributeValue baseValueMin, AttributeValue baseValueMax, Progression progression)
    {
        this.baseValueMin = baseValueMin;
        this.baseValueMax = baseValueMax;
        this.progression = progression;
    }

    /// <summary>
    /// Generates an AttributeValue of this type given a tier.
    /// </summary>
    /// <param name="tier">The tier of affix to be generated</param>
    /// <returns></returns>
    public AttributeValue GetValueForTier(int tier)
    {
        if (progression != null)
        {
            float frac = UnityEngine.Random.Range(0, 100) / 100.0f;
            // Progress min and max values before combining them into a random value between them

            AttributeValue progressedMin = progression(tier, baseValueMin);
            AttributeValue progressedMax = progression(tier, baseValueMax);

            return progressedMin + (progressedMax - progressedMin) * frac;
        }
        Debug.LogError("No progression function set!");
        return null;
    }

    public override string ToString()
    {
        return string.Format("{0} - {1}", baseValueMin, baseValueMax);
    }
}
