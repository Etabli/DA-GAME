using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

/// <summary>
/// Represents information on a single AttributeType. Serializable via a DataContract.
/// </summary>
[DataContract]
public class AttributeInfo
{
    [DataMember]
    public readonly AttributeType Type;
    [DataMember]
    public readonly AttributeValueType ValueType;
    [DataMember]
    public readonly string Name;
    [DataMember]
    public readonly string Description;
    [DataMember]
    public readonly AttributeValueInfo ValueInfo;

    public AttributeInfo(AttributeType type, AttributeValueType attributeClass, string name, AttributeValueInfo valueInfo)
    {
        Type = type;
        ValueType = attributeClass;
        Name = name;
        ValueInfo = valueInfo;

        AttributeInfoDictionary.Add(type, this);
    }

    /// <summary>
    /// Generates an Attribute of this type given a tier.
    /// </summary>
    /// <param name="tier">The tier of the attribute to be generated</param>
    /// <returns></returns>
    public Attribute GenerateAttribute(int tier)
    {
        return new Attribute(Type, ValueType, ValueInfo.GetValueForTier(tier));
    }

    public override string ToString()
    {
        return String.Format("{0}: {1}", Name, ValueInfo);
    }

    #region Static Functionality
    private static Dictionary<AttributeType, AttributeInfo> AttributeInfoDictionary = new Dictionary<AttributeType, AttributeInfo>();

    /// <summary>
    /// Returns the AttributeInfo object for a certain AttributeType, provided it exists
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static AttributeInfo GetAttributeInfo(AttributeType type)
    {
        if (AttributeInfoDictionary.ContainsKey(type))
        {
            return AttributeInfoDictionary[type];
        }
        return null;
    }
    #endregion
}
