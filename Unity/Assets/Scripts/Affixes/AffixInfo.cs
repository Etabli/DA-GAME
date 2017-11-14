using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;
using System.Linq;

/// <summary>
/// Represents information on a single AffixType. Serializable via a DataContract.
/// </summary>
[DataContract]
public class AffixInfo
{
    [DataMember]
    public readonly AffixType Type;
    [DataMember]
    public readonly AffixValueType ValueType;
    [DataMember]
    public readonly string Name;
    [DataMember]
    public readonly string Description;
    [DataMember]
    public readonly AffixValueInfo ValueInfo;

    public AffixInfo(AffixInfo src) : this(src.Type, src.ValueType, src.Name, src.ValueInfo, src.Description)
    { }

    public AffixInfo(AffixType type, AffixValueType valueType, string name, AffixValueInfo valueInfo) : this(type, valueType, name, valueInfo, "")
    { }

    /// <summary>
    /// Creates a new AffixInfo object and adds it to the AffixInfoDictionary
    /// </summary>
    public AffixInfo(AffixType type, AffixValueType valueType, string name, AffixValueInfo valueInfo, string description)
    {
        Type = type;
        ValueType = valueType;
        Name = name;
        Description = description;
        ValueInfo = valueInfo;

        //Debug.Log(string.Format("Adding {0} to dictionary", type));
        AffixInfoDictionary.Add(type, this);
    }

    /// <summary>
    /// Generates an affix of this type given a tier.
    /// </summary>
    /// <param name="tier">The tier of the affix to be generated</param>
    /// <returns></returns>
    public Affix GenerateAffix(int tier)
    {
        return new Affix(Type, ValueType, ValueInfo.GetValueForTier(tier), tier);
    }

    public Affix GenerateAffix(int tier, AffixProgression progression)
    {
        return new Affix(Type, ValueType, ValueInfo.GetValueForTier(tier, progression), tier);
    }

    public override string ToString()
    {
        return string.Format("{0}: " + string.Format(Description, ValueInfo) + " (Type {1}, ValueType {2})", Name, Type, ValueType);
    }

    #region Static Functionality
    private static Dictionary<AffixType, AffixInfo> AffixInfoDictionary = new Dictionary<AffixType, AffixInfo>();

    /// <summary>
    /// Returns the AffixInfo object for a certain AffixType, provided it exists
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static AffixInfo GetAffixInfo(AffixType type)
    {
        // If the requested type is random, select a random affix
        if (type == AffixType.Random)
        {
            System.Random rng = new System.Random();
            return AffixInfoDictionary.ElementAt(rng.Next() % AffixInfoDictionary.Count).Value;
        }

        if (AffixInfoDictionary.ContainsKey(type))
        {
            return AffixInfoDictionary[type];
        }
        return null;
    }
    #endregion
}
