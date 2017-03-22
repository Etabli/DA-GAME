using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

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
    [DataMember]
    protected AttributeProgression progression;

    public AttributeValueInfo(AttributeValue baseValueMin, AttributeValue baseValueMax) : this(baseValueMin, baseValueMax, null)
    { }

    public AttributeValueInfo(AttributeValue baseValueMin, AttributeValue baseValueMax, AttributeProgression progression)
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
        if (!progression.HasProgressionFunction())
        {
            if (!progression.AttachProgressionFunction())
            {
                Debug.LogError(string.Format("Invalid progression function set for AttributeValueInfo of type {0}", baseValueMin.GetType()));
            }
        }
        // At this point we are sure that the progression function is set, but not if parameters are valid

        float frac = UnityEngine.Random.Range(0, 100) / 100.0f;
        // Progress min and max values before combining them into a random value between them

        AttributeValue progressedMin = progression.Apply(baseValueMin, tier);
        if (progression == null)
        {
            // Parameters were invalid
            return null;
        }
        AttributeValue progressedMax = progression.Apply(baseValueMax, tier);

        return progressedMin + (progressedMax - progressedMin) * frac;
    }

    public override string ToString()
    {
        return string.Format("{0} - {1}", baseValueMin, baseValueMax);
    }
}
