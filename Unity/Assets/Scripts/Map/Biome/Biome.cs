using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;




/// <summary>
/// EnemyType Enum
/// IMPORTANT: REMOVE FROM HERE
/// </summary>
public enum EnemyType
{
    Marshmellow,
    Snickers,
    Bat,
    Nutts
}


/// <summary>
/// IMPORTANT: REMOVE FROM HERE
/// </summary>
public enum ResourceType
{
    Asshair,
    Iron,
    Blood,
    Guts
}


/// <summary>
/// Contains all information about a Biome depending on Tier
/// The Resources and Items and Enemies that are in here are the one spawnable in this biome
/// Not all possible spawnable are listed here, because it is restriced by it's tier as well as not every bioem will probably spawn all possible enemies
/// for a biome of this tier
/// </summary>
public class Biome
{

    #region Member Variables
    public BiomeType Type { get; protected set; }
    public int Tier { get; protected set; }
    public List<EnemyType> SpawnableEnemies { get; protected set; }
    public List<ResourceType> SpawnableResource { get; protected set; }
    public List<ItemBaseType> SpawnableItems { get; protected set; }
    #endregion


    #region ctor


    public Biome(BiomeType type, int tier, List<EnemyType> EnemiesSpawnable, List<ResourceType> ResourcesSpawnable, List<ItemBaseType> ItemsSpawnable)
    {
        Type = type;
        Tier = tier;
        SpawnableEnemies = EnemiesSpawnable;
        SpawnableResource = ResourcesSpawnable;
        SpawnableItems = ItemsSpawnable;
    }

    public Biome(BiomeInfo info, int tier)
    {
        Type = info.Type;
        Tier = tier;

        SpawnableEnemies = new List<EnemyType>();
        SpawnableResource = new List<ResourceType>();
        SpawnableItems = new List<ItemBaseType>();

        FillSpawnableEnemies(info.PossibleEnemiesToSpawn);
        FillSpawnableResources(info.PossibleResourceToSpawn);
        FillSpawnableItems(info.PossibleItemsToSpawn);
        //Debug.Log(this.ToString());
    }

    #endregion

    #region Fill Biome Lists

    /// <summary>
    /// Fills the biome spanwable enmeis with random type out of the possible Types for this tier
    /// amount controlled by worldcontroller enemy variaty
    /// </summary>
    /// <param name="possEnemyType"></param>
    void FillSpawnableEnemies(List<EnemyType> possEnemyType)
    {
        Debug.LogWarning("Biome::FillSpwanableEnemies - Currently uses enemies of all types wihtoug checking for tier");
        List<EnemyType> enemiesForThisTier = new List<EnemyType>();
        //= EnemyInfo.GetEnemiesForTier(Tier, possEnemyTpye);  

        System.Random rng = new System.Random((int)System.DateTime.Now.Ticks);


        //TODO: USE enmeies for this tier not poss enemies
        if (WorldController.Instance.EnemyVariatyPerBiome >= possEnemyType.Count)
        {
            SpawnableEnemies = possEnemyType;
            return;
        }

        while (SpawnableEnemies.Count < WorldController.Instance.EnemyVariatyPerBiome)
        {
            //TODO: Use enmeiesFor this tier not poss enemis.
            EnemyType type = possEnemyType[rng.Next(possEnemyType.Count)];
            if (!SpawnableEnemies.Contains(type))
                SpawnableEnemies.Add(type);

        }
    }

    /// <summary>
    /// Fills the spawnable 
    /// </summary>
    /// <param name="possResourceType"></param>
    void FillSpawnableResources(List<ResourceType> possResourceType)
    {
        Debug.LogWarning("Biome::FillSpwanableResources - Currently uses resources of all types wihtoug checking for tier");

        //List<ResourceType> ResourceForThisTier = new List<ResourceType>();
        //= ResourceInfo.GetResourcesForTier(Tier, possEnemyTpye);  

        System.Random rng = new System.Random((int)System.DateTime.Now.Ticks);


        //TODO: USE enmeies for this tier not poss enemies
        if (WorldController.Instance.ResourceVariatyPerBiome >= possResourceType.Count)
        {
            SpawnableResource = possResourceType;
            return;
        }

        while (SpawnableResource.Count < WorldController.Instance.ResourceVariatyPerBiome)
        {
            //TODO: Use enmeiesFor this tier not poss enemis.
            ResourceType type = possResourceType[rng.Next(possResourceType.Count)];
            if (!SpawnableResource.Contains(type))
                SpawnableResource.Add(type);

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="possItemType"></param>
    void FillSpawnableItems(List<ItemBaseType> possItemType)
    {
        Debug.LogWarning("Biome::FillSpwanableItems - Currently uses items of all types wihtoug checking for tier");

        //List<ResourceType> ResourceForThisTier = new List<ResourceType>();
        //= ResourceInfo.GetResourcesForTier(Tier, possEnemyTpye);  

        System.Random rng = new System.Random((int)System.DateTime.Now.Ticks);

        //TODO: USE enmeies for this tier not poss enemies
        if (WorldController.Instance.ItemVariatyPerBiome >= possItemType.Count)
        {
            SpawnableItems = possItemType;
            return;
        }

        while (SpawnableResource.Count < WorldController.Instance.ItemVariatyPerBiome)
        {
            //TODO: Use enmeiesFor this tier not poss enemis.
            ItemBaseType type = possItemType[rng.Next(possItemType.Count)];
            if (!SpawnableItems.Contains(type))
                SpawnableItems.Add(type);
        }
    }


    #endregion


    #region Random Type to Spwan

    /// <summary>
    /// Returns a random enemy type that is spawnable in this biome
    /// </summary>
    /// <returns></returns>
    public EnemyType GetRandomEnemyTypeToSpawn()
    {
        System.Random rng = new System.Random((int)System.DateTime.Now.Ticks);
        return SpawnableEnemies[rng.Next(SpawnableEnemies.Count)];
    }

    /// <summary>
    /// returns a random resource type that is spawnable in this biome
    /// </summary>
    /// <returns></returns>
    public ResourceType GetRandomResourceTypeToSpawn()
    {
        System.Random rng = new System.Random((int)System.DateTime.Now.Ticks);
        return SpawnableResource[rng.Next(SpawnableResource.Count)];

    }

    /// <summary>
    /// returns a random itemType that is spawnable in this biome
    /// </summary>
    /// <returns></returns>
    public ItemBaseType GetRandomItemToSpawn()
    {
        System.Random rng = new System.Random((int)System.DateTime.Now.Ticks);
        return SpawnableItems[rng.Next(SpawnableItems.Count)];

    }
    #endregion

    #region override

    public override string ToString()
    {
        return string.Format("Type: {0}\nTier: {1}\nSpwanable Enemies: {2}\nSpawnable Resources{3}\n Spawnable Items{4}",
            Type,
            Tier,
            string.Join(", ", SpawnableEnemies.Select(e => e.ToString()).ToArray<string>()),
            string.Join(", ", SpawnableResource.Select(e => e.ToString()).ToArray<string>()),
            string.Join(", ", SpawnableItems.Select(e => e.ToString()).ToArray<string>()));
    }

    #endregion
}
