using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

public delegate AttributeValue Progression(int tier, AttributeValue baseValue);

/// <summary>
/// Represents information on an Attribute's value or values. Serialiazable via a DataContract.
/// </summary>
[DataContract]
public class AttributeValueInfo
{
    protected AttributeValue baseValueMin;
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
            float frac = Random.Range(0, 100) / 100.0f;
            // Progress min and max values before combining them into a random value between them
            Debug.Log(string.Format("Generating Attribute of type {0} with base range {1} - {2}", baseValueMin.GetType(), baseValueMin, baseValueMax));
            Debug.Log(string.Format("Chosen fraction was {0}, and tier is {1}", frac, tier));

            AttributeValue progressedMin = progression(tier, baseValueMin);
            AttributeValue progressedMax = progression(tier, baseValueMax);
            Debug.Log(string.Format("Progressed range is {0} - {1}", progressedMin, progressedMax));

            AttributeValue progressedDifference = progressedMax - progressedMin;
            Debug.Log(string.Format("Progressed difference is {0}", progressedDifference));

            return progressedMin + (progressedMax - progressedMin) * frac;
        }
        Debug.LogError("No progression function set!");
        return null;
    }
}
