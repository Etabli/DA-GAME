using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System;
using System.Xml;
using System.Text;

public enum AffixPoolPreset
{
    Weapon,
    Armor,
    Ammo
}

/// <summary>
/// Represents a pool of AffixTypes that can be randomly drawn from with the option
/// to immediately generate the Affixes. In that case either a fixed tier or tier
/// lottery can be used.
/// </summary>
[DataContract]
[KnownType(typeof(Lottery<AffixType>))]
public class AffixPool
{
    [DataMember]
    protected Lottery<AffixType> lottery;

    public AffixPool(params AffixType[] types)
    {
        lottery = new Lottery<AffixType>();
        foreach (AffixType t in types)
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

    public void CombineInto(AffixPool pool)
    {
        lottery.CombineInto(pool.lottery);
    }

    /// <summary>
    /// Returns a random AffixType in this pool
    /// </summary>
    /// <returns></returns>
    public AffixType GetRandomType()
    {
        return lottery.Draw();
    }

    /// <summary>
    /// Returns a list of random AffixTypes in this pool
    /// </summary>
    /// <param name="n">The number of AffixTypes to draw</param>
    /// <returns></returns>
    public AffixType[] GetRandomTypes(int n)
    {
        List<AffixType> types = new List<AffixType>();
        for (int i = 0; i < n; i++)
        {
            types.Add(GetRandomType());
        }
        return types.ToArray();
    }

    /// <summary>
    /// Returns a list of random AffixTypes in this pool, making sure that they are all unique.
    /// If the number of AffixTypes to draw exceeds the number of available AffixTypes, all of them are returned in a random order
    /// </summary>
    /// <param name="n">The number of AffixTypes to draw</param>
    /// <returns></returns>
    public AffixType[] GetUniqueRandomTypes(int n)
    {
        lottery.StartBatchDraw();
        AffixType[] types = lottery.Draw(n).ToArray();
        lottery.EndBatchDraw();
        return types;
    }

    /// <summary>
    /// Returns a random Affix of a type in this pool. The Affix will be of the given tier.
    /// </summary>
    /// <returns></returns>
    public Affix GetRandomAffix(int tier)
    {
        AffixType type = GetRandomType();
        return AffixInfo.GetAffixInfo(type).GenerateAffix(tier);
    }

    /// <summary>
    /// Returns a list of random Affixes of types in this pool. All the Affixes will be of the given tier.
    /// </summary>
    /// <param name="n">The number of Affixes to draw</param>
    /// <returns></returns>
    public Affix[] GetRandomAffixes(int n, int tier)
    {
        List<Affix> affixes = new List<Affix>();

        AffixType[] types = GetRandomTypes(n);
        foreach (AffixType type in types)
        {
            affixes.Add(AffixInfo.GetAffixInfo(type).GenerateAffix(tier));
        }
        return affixes.ToArray();
    }

    /// <summary>
    /// Returns a list of random Affixes of types in this pool, making sure each is of a different type. All Affixes will be of the given tier.
    /// </summary>
    /// <param name="n">The number of Affixes to draw</param>
    /// <returns></returns>
    public Affix[] GetUniqueRandomAffixes(int n, int tier)
    {
        List<Affix> affixes = new List<Affix>();

        AffixType[] types = GetUniqueRandomTypes(n);
        foreach (AffixType type in types)
        {
            affixes.Add(AffixInfo.GetAffixInfo(type).GenerateAffix(tier));
        }
        return affixes.ToArray();
    }

    /// <summary>
    /// Returns a list of random affixes in this pool, making sure they are all of different types. The tiers will be determined through the provided Lottery.
    /// </summary>
    /// <param name="totalTiers">The total number of tiers of affixes to be generated</param>
    /// <param name="tierDistribution">The distribution of tiers</param>
    /// <returns></returns>
    public Affix[] GetUniqueRandomAffixes(int totalTiers, Lottery<int> tierDistribution, HashSet<AffixType> blacklist)
    {
        List<Affix> affixes = new List<Affix>();

        lottery.StartBatchDraw(blacklist);
        while (totalTiers > 0)
        {
            // Get the tier of the Affix to be generated and cap it if it exceeds the total number of tiers left
            int tier = tierDistribution.Draw();
            tier = tier > totalTiers ? totalTiers : tier;

            AffixType type = lottery.Draw();
            if (type == AffixType.None)
            {
                // We've run out of AffixTypes to draw from
                break;
            }

            affixes.Add(AffixInfo.GetAffixInfo(type).GenerateAffix(tier));
            totalTiers -= tier;
        }
        lottery.EndBatchDraw();

        return affixes.ToArray();
    }

    public Affix[] GetUniqueRandomAffixes(int totalTiers, Lottery<int> tierDistribution)
    {
        return GetUniqueRandomAffixes(totalTiers, tierDistribution, new HashSet<AffixType>());
    }

    public override string ToString()
    {
        return lottery.ToString();
    }

    #region Static Functionality

    private static Dictionary<AffixPoolPreset, AffixPool> PresetPools = new Dictionary<AffixPoolPreset, AffixPool>();

    public static void RegisterPresetPool(AffixPoolPreset preset, AffixPool pool)
    {
        PresetPools[preset] = pool;
    }

    public static void RegisterPresetPool(AffixPoolPreset preset, params AffixType[] types)
    {
        RegisterPresetPool(preset, new AffixPool(types));
    }

    public static AffixPool GetPool(AffixPoolPreset preset)
    {
        if (PresetPools.ContainsKey(preset))
        {
            return PresetPools[preset];
        }
        return null;
    }

    public static AffixPool GetPool(params AffixPoolPreset[] presets)
    {
        AffixPool result = new AffixPool();
        foreach (AffixPoolPreset p in presets)
        {
            PresetPools[p].CombineInto(result);
        }
        return result;
    }

    public static void SavePresets()
    {
        Serializer.SaveAffixPoolsToDisk(PresetPools);
    }

    public static void LoadPresets()
    {
        PresetPools = Serializer.LoadAffixPoolsFromDisk();
    }
    #endregion
}
