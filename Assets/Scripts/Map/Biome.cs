﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The possible bioms
/// </summary>
public enum BiomeType
{
    Grass,
    Swamp,
    DickGrease
}

/// <summary>
/// EnemyType Enum
/// IMPORTANT: REMOVE FROM HERE
/// </summary>
public enum EnemyType
{
    Marshmellow
}

/// <summary>
/// IMPORTANT: REMOVE FROM HERE
/// </summary>
public enum ItemType
{
   Cuntjuice
}

/// <summary>
/// IMPORTANT: REMOVE FROM HERE
/// </summary>
public enum ResourceType
{
    Asshair
}

public static class Biome
{

    public static Dictionary<BiomeType, List<EnemyType>> SpawnableEnemyTypeDictionary = new Dictionary<BiomeType, List<EnemyType>>();
    public static Dictionary<BiomeType, List<ItemType>> SpwanableItemTypeDictionary = new Dictionary<BiomeType, List<ItemType>>();
    public static Dictionary<BiomeType, List<ResourceType>> SpwanableResourcesDictionary = new Dictionary<BiomeType, List<ResourceType>>();

    public static Dictionary<BiomeType, Tuple<int, int>> MinMaxTierDictionray = new Dictionary<BiomeType, Tuple<int, int>>();

    #region Spawnables
    /// <summary>
    /// This function returns the List of spwanable EnemyTypes for a given biome type
    /// </summary>
    /// <param name="biome">The BiomeType</param>
    /// <returns>The list of possible spwanable enemy types</returns>
    public static List<EnemyType> GetSpawnableEnemies(BiomeType biome)
    {
        if(SpawnableEnemyTypeDictionary.ContainsKey(biome))
        {
            return SpawnableEnemyTypeDictionary[biome];
        }
        Debug.Log("Biome:: GetSpawnableEnemies - There is no entry for this BiomeType");
        return null;
    }

    /// <summary>
    /// Returnes the list of spawnable Items for a given biome type
    /// </summary>
    /// <param name="biome">The BiomeType</param>
    /// <returns>The list of spawnable items</returns>
    public static List<ItemType> GetSpwanableItems(BiomeType biome)
    {
        if(SpwanableItemTypeDictionary.ContainsKey(biome))
        {
            return SpwanableItemTypeDictionary[biome];
        }
        Debug.Log("Biome:: GetSpwanableItems - There is no entry for this BiomeType");
        return null;
    }


    /// <summary>
    /// Returns the possible spawnable resources for a given biome type
    /// </summary>
    /// <param name="biome">The BiomeType</param>
    /// <returns>The list of spawnable items </returns>
    public static List<ResourceType> GetSpwanableResources(BiomeType biome)
    {
        if(SpwanableResourcesDictionary.ContainsKey(biome))
        {
            return SpwanableResourcesDictionary[biome];
        }
        Debug.Log("Biome:: GetSpwanableResources - There is no entry for this BiomeType");
        return null;
    }
    #endregion


    #region Tier
    /// <summary>
    /// Returns the min and max tier, in which a biome is spawnable
    /// The first entry is the min tier, the second the max tier
    /// </summary>
    /// <param name="biome">The BiomeType</param>
    /// <returns>A tuple with the min and max tier.</returns>
    public static Tuple<int,int> GetMinMaxTier(BiomeType biome)
    {
        if(MinMaxTierDictionray.ContainsKey(biome))
        {
            return MinMaxTierDictionray[biome];
        }
        Debug.Log("Biome:: GetMinMaxTier - There is no entry for this BiomeType");
        return null;
    }
    /// <summary>
    /// Returns the minimum tier,in which a bioem is spawnable 
    /// </summary>
    /// <param name="biome">The BiomeType</param>
    /// <returns>The minimum tier, -1 when there is no entry for this BiomeType</returns>
    public static int GetMinTier(BiomeType biome)
    {
        if(MinMaxTierDictionray.ContainsKey(biome))
        {
            return MinMaxTierDictionray[biome].Item1;
        }
        Debug.Log("Biome:: GetMinTier - There is no entry for this BiomeType");
        return -1;
    }

    /// <summary>
    /// Returns the maximum  tier, in which a biome is spawnable
    /// </summary>
    /// <param name="biome">The BiomeType</param>
    /// <returns>The maximum  tier, -1 when there is no entry for this BiomeType</returns>
    public static int GetMaxTier(BiomeType biome)
    {
        if(MinMaxTierDictionray.ContainsKey(biome))
        {
            return MinMaxTierDictionray[biome].Item2;
        }
        Debug.Log("Biome:: GetMaxTier - There is no entry for this BiomeType");
        return -1;
    }
    #endregion


    #region Loading and Saving
    /// <summary>
    /// Loads the information for every biome on start up, most likely form a file
    /// Liekely DataOrganization:
    /// BiomeType: Enemies
    ///            Items
    ///            Resources
    ///            MinMaxTier
    ///  BiomeType:
    ///             .....
    /// </summary>
    public static void LoadBiomeInfromation()
    {
        Debug.LogError("Biome::LoadInformationForBiomes -  Not Implemented");
        //TODO: Load the biome data
    }


    public static void SaveBiomeInformation()
    {
        Debug.LogError("Biome::SaveBiomeInformation -  Not Implemented");
        //TODO: Load the biome data
    }

    #endregion


    #region GenerateBiomeLandscape
    
    //TODO: Figure out how to generate landscape 
    //      Create Function that calls landscape generation function based on biometype
    //      Fidddles with params or create different functions per biome

    #endregion


}
