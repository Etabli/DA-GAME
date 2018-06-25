using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization;
using Newtonsoft.Json;

// TODO: Change saving
public abstract class Serializer
{
    const string DATA_FOLDER_PATH = "Data/";
    const string AFFIX_FOLDER_PATH = DATA_FOLDER_PATH + "Affix/";
    const string AFFIX_INFO_FOLDER_PATH = AFFIX_FOLDER_PATH + "AffixInfo/";
    const string AFFIX_POOL_FILE_PATH = AFFIX_FOLDER_PATH + "AffixPools";
    const string AFFIX_TYPE_FILE_PATH = AFFIX_FOLDER_PATH + "AffixTypes";
    const string BIOME_FOLDER_PATH = DATA_FOLDER_PATH + "Biome/";

    public static void Initialize()
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            Converters = new List<JsonConverter> { new AffixPoolJsonConverter() }
        };
    }

    #region AffixInfo
    /// <summary>
    /// Takes an AffixInfo object and returns a string containing formatted XML describing it.
    /// </summary>
    /// <param name="info">The AffixInfo object to be serialized</param>
    /// <returns>A strong containing formatted XML</returns>
    public static string SerializeAffixInfo(AffixInfo info)
    {
        return JsonConvert.SerializeObject(info);
    }

    /// <summary>
    /// Deserializes an Affix Info object from a string. Automatically calls its constructor to perform setup work.
    /// </summary>
    /// <param name="data">The XML string describing the AffixInfo object</param>
    /// <returns>The usable AffixInfo object</returns>
    public static AffixInfo DeserializeAffixInfo(string data)
    {
        return JsonConvert.DeserializeObject<AffixInfo>(data);
    }

    /// <summary>
    /// Saves info about a certain affix type to disk.
    /// </summary>
    public static void SaveAffixInfoToDisk(AffixInfo info)
    {
        SaveAffixInfoToDisk(info, GetSystemPath(GetPathFromAffixType(info.Type)));
    }

    /// <summary>
    /// Saves an AffixInfo to disk at a specified location.
    /// </summary>
    /// <param name="path">The full path, including filename</param>
    public static void SaveAffixInfoToDisk(AffixInfo info, string path)
    {
        string data = SerializeAffixInfo(info);

        using (FileStream file = new FileStream(path, FileMode.Create))
        using (StreamWriter writer = new StreamWriter(file))
            writer.Write(data);
    }

    /// <summary>
    /// Loads info about a certain affix type at disk
    /// </summary>
    /// <returns>The newly constructed AffixInfo object.</returns>
    public static AffixInfo LoadAffixInfoFromDisk(AffixType type)
    {
        return LoadAffixInfoFromDisk(GetPathFromAffixType(type));
    }

    /// <summary>
    /// Loads the affix info file at the specified location in the resources folder
    /// </summary>
    /// <returns>The newly constructed AffixInfo object.</returns>
    public static AffixInfo LoadAffixInfoFromDisk(string path)
    {
        TextAsset text = Resources.Load<TextAsset>(path);
        if (text == null)
            throw new ArgumentException($"'{path}' is not a valid text asset!");

        return DeserializeAffixInfo(text.text);
    }

    /// <summary>
    /// Loads all affix infos from disk
    /// </summary>
    public static void LoadAllAffixInfosFromDisk()
    {
        LoadAllAffixInfosFromDisk(AFFIX_INFO_FOLDER_PATH);
    }

    /// <summary>
    /// Loads all affix infos from disk and registers them in the affix info dictionary
    /// </summary>
    /// <param name="folder">The path to the root data folder</param>
    public static void LoadAllAffixInfosFromDisk(string folder)
    {
        var texts = Resources.LoadAll<TextAsset>(folder);
        foreach (var text in texts)
        {
            AffixInfo.Register(DeserializeAffixInfo(text.text));
        }
    }

    /// <summary>
    /// Deletes the info file for a specific type
    /// </summary>
    /// <param name="type"></param>
    public static void DeleteAffixInfo(AffixType type)
    {
        File.Delete(GetSystemPath(GetPathFromAffixType(type)));
    }

    private static string GetFileNameFromAffixType(AffixType type)
    {
        return type.ToString();
    }

    private static string GetPathFromAffixType(AffixType type)
    {
        return AFFIX_INFO_FOLDER_PATH + GetFileNameFromAffixType(type);
    }
    #endregion

    #region AffixPool
    public static void SaveAffixPoolsToDisk(Dictionary<AffixPoolPreset, AffixPool> pools)
    {
        using (FileStream file = new FileStream(GetSystemPath(AFFIX_POOL_FILE_PATH), FileMode.Create))
        using (StreamWriter sw = new StreamWriter(file))
            sw.Write(JsonConvert.SerializeObject(pools));
    }

    public static Dictionary<AffixPoolPreset, AffixPool> LoadAffixPoolsFromDisk()
    {
        TextAsset text = Resources.Load<TextAsset>(AFFIX_POOL_FILE_PATH);
        if (text == null)
            throw new ArgumentException($"{AFFIX_POOL_FILE_PATH} is not a valid text asset!");

        return JsonConvert.DeserializeObject<Dictionary<AffixPoolPreset, AffixPool>>(text.text);
    }
    #endregion

    #region BiomeInfo
    public static string SerializeBiomeInfo(BiomeInfo info)
    {
        return JsonConvert.SerializeObject(info);
    }

    /// <summary>
    /// deserializes a biome info
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static BiomeInfo DeserializeBiomeInfo(string data)
    {
        return JsonConvert.DeserializeObject<BiomeInfo>(data);
    }

    /// <summary>
    /// Save Biome Info to disk
    /// </summary>
    public static void SaveBiomeInfoToDisk(BiomeInfo info)
    {
        string data = SerializeBiomeInfo(info);

        //Debug.Log(data);

        using (FileStream file = new FileStream(GetSystemPath(GetBiomePathFromType(info.Type)), FileMode.Create))
        using (StreamWriter writer = new StreamWriter(file))
            writer.Write(data);
    }

    /// <summary>
    /// Loads the information about a biome from disk
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static BiomeInfo LoadBiomeInfoFromDisk(BiomeType type)
    {
        TextAsset text = Resources.Load<TextAsset>(GetBiomePathFromType(type));
        if (text == null)
            throw new ArgumentException($"Couldn't find info file for biome type {type}");

        string data;
        using (MemoryStream stream = new MemoryStream(text.bytes))
        using (StreamReader reader = new StreamReader(stream))
            data = reader.ReadToEnd();

        return new BiomeInfo(DeserializeBiomeInfo(data));
    }

    /// <summary>
    /// Loads all biome infos form a disk
    /// </summary>
    public static void LoadAllBiomeInfosFromDisk()
    {
        foreach (BiomeType type in System.Enum.GetValues(typeof(BiomeType)))
        {
            LoadBiomeInfoFromDisk(type);
        }
    }

    /// <summary>
    /// Saves all biomes to the disk
    /// </summary>
    public static void SaveAllBiomeInfoToDisk()
    {
        foreach (BiomeType type in System.Enum.GetValues(typeof(BiomeType)))
        {
            SaveBiomeInfoToDisk(BiomeInfo.GetBiomeInfo(type));
        }
    }

    /// <summary>
    /// returns a path to location of the file depending on biome type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetBiomePathFromType(BiomeType type)
    {
        return BIOME_FOLDER_PATH + type;
    }
    #endregion

    /// <summary>
    /// Takes a path that could be used to load an xml file via Resources.Load and
    /// turns it into a path usable by System.IO.
    /// </summary>
    /// <param name="resourcePath"></param>
    /// <returns></returns>
    private static string GetSystemPath(string resourcePath)
    {
        return "Assets/Resources/" + resourcePath + ".json";
    }
}

public class AffixPoolJsonConverter : JsonConverter<AffixPool>
{
    public override void WriteJson(JsonWriter writer, AffixPool value, JsonSerializer serializer)
    {
        AffixPool pool = value as AffixPool;

        List<string> names = new List<string>(pool.AffixTypes.Count);
        foreach (var type in pool.AffixTypes)
        {
            names.Add(type.Name);
        }
        
        serializer.Serialize(writer, names);
    }

    public override AffixPool ReadJson(JsonReader reader, Type objectType, AffixPool existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        List<string> names = serializer.Deserialize<List<string>>(reader);

        AffixType[] types = new AffixType[names.Count];
        for (int i = 0; i < names.Count; i++)
        {
            types[i] = AffixType.GetByName(names[i]);
        }

        return new AffixPool(types);
    }
}

