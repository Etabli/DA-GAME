using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Linq;
using System;

/// <summary>
/// The possible bioms
/// </summary>
public enum BiomeType
{
    Grass,
    Swamp,
    House,
    Ice,
    DickGrease
}
/// <summary>
/// Represents the information about one BiomeType
/// </summary>
[DataContract]
public class BiomeInfo{

    #region Memeber variables
    [DataMember]
    public BiomeType Type { get; protected set; }
    [DataMember]
    public List<EnemyType> PossibleEnemiesToSpawn { get; protected set; }
    [DataMember]
    public List<ItemBaseType> PossibleItemsToSpawn { get; protected set; }
    [DataMember]
    public List<ResourceType> PossibleResourceToSpawn { get; protected set; }
    [DataMember]
    Range MinMaxTier;

    #endregion

    #region ctor
    public BiomeInfo(BiomeType type, List<EnemyType>enemyList, List<ItemBaseType> itemList, List<ResourceType> resourceList,Range minMaxTier)
    {
        Type = type;
        PossibleEnemiesToSpawn = enemyList;
        PossibleItemsToSpawn = itemList;
        PossibleResourceToSpawn = resourceList;
        MinMaxTier = minMaxTier;
        //Debug.Log("Created Biome Info for type " + Type);
        if(!BiomeInfoDictionary.ContainsKey(Type))
        {
            //Debug.Log("Adding BiomeTpye " + Type + " to biomeinfo dictionary");
            BiomeInfoDictionary.Add(Type, this);
        }
        else if(BiomeInfoDictionary[type] == null)
        {
            //Debug.Log("Updating BiomeTpye " + Type + " to biomeinfo dictionary");
            BiomeInfoDictionary[type] = this;
        }
    }

    public BiomeInfo(BiomeType type, List<EnemyType> enemyList, List<ItemBaseType> itemList, List<ResourceType> resourceList,int minTier, int maxtier):
        this(type,enemyList,itemList,resourceList,new Range((float)minTier,(float)maxtier))
    { }

    public BiomeInfo(BiomeInfo src):this(src.Type,src.PossibleEnemiesToSpawn,src.PossibleItemsToSpawn,src.PossibleResourceToSpawn,src.MinMaxTier)
    { }

    #endregion

    #region overrides

    public override string ToString()
    {

        string possEnemies = string.Join(", ", PossibleEnemiesToSpawn.Select(e => e.ToString()).ToArray<string>());
        string possibleItems = string.Join(", ", PossibleItemsToSpawn.Select(e => e.ToString()).ToArray<string>());
        string possResources = string.Join(", ", PossibleResourceToSpawn.Select(e => e.ToString()).ToArray<string>());
        return string.Format("Type: {0}\nPossible Enemies: {1}\nPossible Items: {2}\nPossible Resources: {3}\nMinimum and Maximum Tier: {4}"
                            ,Type.ToString(),possEnemies,possibleItems,possResources,MinMaxTier.ToString());
    }

    #endregion

    #region static Functionality

    private static Dictionary<BiomeType, BiomeInfo> BiomeInfoDictionary = new Dictionary<BiomeType, BiomeInfo>();

    /// <summary>
    /// Returnes the BiomeInfo for a Type
    /// </summary>
    /// <param name="type">The wanted BiomeType</param>
    /// <returns>The BiomeInfo if it exists</returns>
    public static BiomeInfo GetBiomeInfo(BiomeType type)
    {
        if(BiomeInfoDictionary.ContainsKey(type))
        {
            return BiomeInfoDictionary[type];
        }
        return null;
    }

    /// <summary>
    /// Returns all possible BiomeTypes depending on Tier
    /// </summary>
    /// <param name="tier"></param>
    /// <returns>All possible biomesalle</returns>
    public static List<BiomeType> GetPossibleBiomesForTier(int tier)
    {
        List<BiomeType> possBiomes = new List<BiomeType>();

        foreach (BiomeType type in BiomeInfoDictionary.Keys)
        {
            if(BiomeInfoDictionary[type] != null)
            {
                if (BiomeInfoDictionary[type].MinMaxTier.IsInRange(tier))
                {
                    possBiomes.Add(type);
                }
            }
        }
               
        return possBiomes;
    }

    #endregion
}
