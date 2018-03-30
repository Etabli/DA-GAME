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
    public readonly string Description;
    [DataMember]
    public readonly AffixValueInfo ValueInfo;

    public int ID { get { return Type.ID; } }
    public string Name { get { return Type.Name; } }

    /// <summary>
    /// Copies from an existing affix info object. Note that this also recreates the affix type.
    /// </summary>
    public AffixInfo(AffixInfo src) : this(src.ID, src.Name, src.ValueInfo, src.Description)
    { }

    /// <summary>
    /// Creates a new AffixInfo object and its associated type
    /// </summary>
    public AffixInfo(string name, AffixValueInfo valueInfo) : this(name, valueInfo, "")
    { }

    /// <summary>
    /// Creates a new AffixInfo object and its associated type
    /// </summary>
    public AffixInfo(string name, AffixValueInfo valueInfo, string description)
        : this(AffixType.CreateNew(name), valueInfo, description)
    { }

    /// <summary>
    /// Creates a new AffixInfo object and its associated type
    /// </summary>
    public AffixInfo(int id, string name, AffixValueInfo valueInfo, string description)
        : this(AffixType.Replace(id, name), valueInfo, description)
    { }

    /// <summary>
    /// Creates a new AffixInfo object with a preexisting type
    /// </summary>
    public AffixInfo(AffixType type, AffixValueInfo valueInfo, string description)
        : this(valueInfo, description)
    {
        Type = type;
    }

    private AffixInfo(AffixValueInfo valueInfo, string description)
    {
        Description = description;
        ValueInfo = new AffixValueInfo(valueInfo);
    }

    /// <summary>
    /// Generates an affix of this type given a tier.
    /// </summary>
    public Affix GenerateAffix(int tier)
    {
        return new Affix(Type, ValueInfo.GetValueForTier(tier), tier);
    }

    public override string ToString()
    {
        return string.Format("{0}: " + string.Format(Description, ValueInfo) + " (Type {1})", Name, Type);
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
    /// Deregisters an affix type from the affix info dictionary
    /// </summary>
    /// <param name="type"></param>
    public static void Deregister(AffixType type)
    {
        if (affixInfoDictionary.ContainsKey(type))
            affixInfoDictionary.Remove(type);
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
