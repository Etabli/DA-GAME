using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System;
using System.Linq;
using UnityEngine;

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
    public readonly string Description;
    [DataMember]
    public readonly AffixValueInfo ValueInfo;

    public int ID { get { return Type.ID; } }
    public string Name { get { return Type.Name; } }

    public AffixInfo(AffixInfo src) : this(src.ID, src.Name, src.ValueType, src.ValueInfo, src.Description)
    { }

    public AffixInfo(string name, AffixValueType valueType, AffixValueInfo valueInfo) : this(name, valueType, valueInfo, "")
    { }

    /// <summary>
    /// Creates a new AffixInfo object
    /// </summary>
    public AffixInfo(string name, AffixValueType valueType, AffixValueInfo valueInfo, string description)
        : this(valueType, valueInfo, description)
    {
        Type = AffixType.CreateNew(name);
    }

    public AffixInfo(int id, string name, AffixValueType valueType, AffixValueInfo valueInfo, string description)
        : this(valueType, valueInfo, description)
    {
        Type = AffixType.Replace(id, name);
    }

    private AffixInfo(AffixValueType valueType, AffixValueInfo valueInfo, string description)
    {
        ValueType = valueType;
        Description = description;
        ValueInfo = new AffixValueInfo(valueInfo);
    }

    /// <summary>
    /// Generates an affix of this type given a tier.
    /// </summary>
    public Affix GenerateAffix(int tier)
    {
        return new Affix(Type, ValueType, ValueInfo.GetValueForTier(tier), tier);
    }

    /// <summary>
    /// Generates an affix of this type with a non-default progression function.
    /// </summary>
    public Affix GenerateAffix(int tier, AffixProgression progression)
    {
        return new Affix(Type, ValueType, ValueInfo.GetValueForTier(tier, progression), tier);
    }

    public override string ToString()
    {
        return string.Format("{0}: " + string.Format(Description, ValueInfo) + " (Type {1}, ValueType {2})", Name, Type, ValueType);
    }

    #region Static Functionality
    private static Dictionary<AffixType, AffixInfo> affixInfoDictionary = new Dictionary<AffixType, AffixInfo>();

    /// <summary>
    /// Registers an AffixInfo object in the affix info dictionary
    /// </summary>
    /// <param name="info"></param>
    public static void Register(AffixInfo info)
    {
        affixInfoDictionary[info.Type] = info;
    }

    /// <summary>
    /// Returns the AffixInfo object for a certain AffixType, provided it exists
    /// </summary>
    public static AffixInfo GetAffixInfo(AffixType type)
    {
        // If the requested type is random, select a random affix
        if (type == AffixType.Random)
        {
            System.Random rng = new System.Random();
            return affixInfoDictionary.ElementAt(rng.Next() % affixInfoDictionary.Count).Value;
        }

        if (affixInfoDictionary.ContainsKey(type))
        {
            return affixInfoDictionary[type];
        }
        return null;
    }

    /// <summary>
    /// Generates an affix of the given type and tier.
    /// </summary>
    public static Affix GenerateAffix(AffixType type, int tier)
    {
        return GetAffixInfo(type).GenerateAffix(tier);
    }
    #endregion
}
