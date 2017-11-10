﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.IO;
using System;
using System.Xml;
using System.Text;

public enum AttributePoolPreset
{
    Weapon,
    Armor,
    Ammo
}

/// <summary>
/// Represents a pool of AttributeTypes that can be randomly drawn from with the option
/// to immediately generate the Attributes. In that case either a fixed tier or tier
/// lottery can be used.
/// </summary>
[DataContract]
[KnownType(typeof(Lottery<AttributeType>))]
public class AttributePool
{
    [DataMember]
    protected Lottery<AttributeType> lottery;

    public AttributePool(params AttributeType[] types)
    {
        lottery = new Lottery<AttributeType>();
        foreach (AttributeType t in types)
        {
            lottery.Enter(t, 1);
        }
    }

    /// <summary>
    /// Changes the seed of the internal lottery to its default value.
    /// </summary>
    public void ChangeSeed()
    {
        lottery.ChangeSeed();
    }

    /// <summary>
    /// Changes the seed of the internal lottery.
    /// </summary>
    /// <param name="seed">The seed to change to</param>
    public void ChangeSeed(int seed)
    {
        lottery.ChangeSeed(seed);
    }

    public void CombineInto(AttributePool pool)
    {
        lottery.CombineInto(pool.lottery, EntryOptions.Discard);
    }
    
    /// <summary>
    /// Returns a random AttributeType in this pool
    /// </summary>
    /// <returns></returns>
    public AttributeType GetRandomType()
    {
        return lottery.GetWinner();
    }

    /// <summary>
    /// Returns a list of random AttributeTypes in this pool
    /// </summary>
    /// <param name="n">The number of AttributeTypes to draw</param>
    /// <returns></returns>
    public AttributeType[] GetRandomTypes(int n)
    {
        List<AttributeType> types = new List<AttributeType>();
        for (int i = 0; i < n; i++)
        {
            types.Add(GetRandomType());
        }
        return types.ToArray();
    }

    /// <summary>
    /// Returns a list of random AttributeTypes in this pool, making sure that they are all unique.
    /// If the number of AttributeTypes to draw exceeds the number of available AttributeTypes, all of them are returned in a random order
    /// </summary>
    /// <param name="n">The number of AttributeTypes to draw</param>
    /// <returns></returns>
    public AttributeType[] GetUniqueRandomTypes(int n)
    {
        lottery.StartBatchDraw();
        AttributeType[] types = lottery.DrawBatch(n);
        lottery.EndBatchDraw();
        return types;
    }

    /// <summary>
    /// Returns a random Attribute of a type in this pool. The attribute will be of the given tier.
    /// </summary>
    /// <returns></returns>
    public Attribute GetRandomAttribute(int tier)
    {
        AttributeType type = GetRandomType();
        return AttributeInfo.GetAttributeInfo(type).GenerateAttribute(tier);
    }

    /// <summary>
    /// Returns a list of random Attributes of types in this pool. All the attributes will be of the given tier.
    /// </summary>
    /// <param name="n">The number of attributes to draw</param>
    /// <returns></returns>
    public Attribute[] GetRandomAttributes(int n, int tier)
    {
        List<Attribute> attributes = new List<Attribute>();

        AttributeType[] types = GetRandomTypes(n);
        foreach (AttributeType type in types)
        {
            attributes.Add(AttributeInfo.GetAttributeInfo(type).GenerateAttribute(tier));
        }
        return attributes.ToArray();
    }

    /// <summary>
    /// Returns a list of random Attributes of types in this pool, making sure each is of a different type. All attributes will be of the given tier.
    /// </summary>
    /// <param name="n">The number of attributes to draw</param>
    /// <returns></returns>
    public Attribute[] GetUniqueRandomAttributes(int n, int tier)
    {
        List<Attribute> attributes = new List<Attribute>();

        AttributeType[] types = GetUniqueRandomTypes(n);
        foreach (AttributeType type in types)
        {
            attributes.Add(AttributeInfo.GetAttributeInfo(type).GenerateAttribute(tier));
        }
        return attributes.ToArray();
    }

    /// <summary>
    /// Returns a list of random attributes in this pool, making sure they are all of different types. The tiers will be determined through the provided Lottery.
    /// </summary>
    /// <param name="totalTiers">The total number of tiers of attributes to be generated</param>
    /// <param name="tierDistribution">The distribution of tiers</param>
    /// <returns></returns>
    public Attribute[] GetUniqueRandomAttributes(int totalTiers, Lottery<int> tierDistribution, HashSet<AttributeType> blacklist)
    {
        List<Attribute> attributes = new List<Attribute>();

        lottery.StartBatchDraw(blacklist);
        while (totalTiers > 0)
        {
            // Get the tier of the attribute to be generated and cap it if it exceeds the total number of tiers left
            int tier = tierDistribution.Draw();
            tier = tier > totalTiers ? totalTiers : tier;

            AttributeType type = lottery.DrawBatch();
            if (type == AttributeType.None)
            {
                // We've run out of AttributeTypes to draw from
                Debug.Log("Ran out of attribute types while drawing from pool");
                break;
            }

            attributes.Add(AttributeInfo.GetAttributeInfo(type).GenerateAttribute(tier));
            totalTiers -= tier;
        }
        lottery.EndBatchDraw();

        return attributes.ToArray();
    }

    public Attribute[] GetUniqueRandomAttributes(int totalTiers, Lottery<int> tierDistribution)
    {
        return GetUniqueRandomAttributes(totalTiers, tierDistribution, new HashSet<AttributeType>());
    }

    public override string ToString()
    {
        return lottery.ToString();
    }

    #region Static Functionality
    public const string SAVE_FILE_NAME = "AttributePools";

    private static Dictionary<AttributePoolPreset, AttributePool> PresetPools = new Dictionary<AttributePoolPreset, AttributePool>();

    public static void RegisterPresetPool(AttributePoolPreset preset, AttributePool pool)
    {
        if (PresetPools.ContainsKey(preset))
        {
            Debug.LogError(string.Format("AttributePool for slot {0} is already assigned!", preset));
            return;
        }
        PresetPools[preset] = pool;
    }

    public static void RegisterPresetPool(AttributePoolPreset preset, params AttributeType[] types)
    {
        RegisterPresetPool(preset, new AttributePool(types));
    }

    public static AttributePool GetPool(AttributePoolPreset preset)
    {
        if (PresetPools.ContainsKey(preset))
        {
            return PresetPools[preset];
        }
        Debug.LogError(string.Format("No AttributePool registered for slot {0}!", preset));
        return null;
    }

    public static AttributePool GetPool(params AttributePoolPreset[] presets)
    {
        AttributePool result = new AttributePool();
        foreach (AttributePoolPreset p in presets)
        {
            PresetPools[p].CombineInto(result);
        }
        return result;
    }

    public static void SavePresets()
    {
        Serializer.SaveAttributePoolsToDisk(PresetPools);
    }

    public static void LoadPresets()
    {
        PresetPools = Serializer.LoadAttributePoolsFromDisk();
    }
    #endregion
}
