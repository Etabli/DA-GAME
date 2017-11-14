using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using UnityEngine;

abstract class Serializer
{
    const string DATA_FOLDER_PATH = "Data\\";
    const string AFFIX_FOLDER_PATH = DATA_FOLDER_PATH + "Affix\\";
    const string AFFIX_INFO_FOLDER_PATH = AFFIX_FOLDER_PATH + "AffixInfo\\";
    const string AFFIX_POOL_FILE_PATH = AFFIX_FOLDER_PATH + "AffixPools";
    const string BIOME_FOLDER_PATH = DATA_FOLDER_PATH + "Biome\\";

    #region AffixInfo
    /// <summary>
    /// Takes an AffixInfo object and returns a string containing formatted XML describing it
    /// </summary>
    /// <param name="info">The AffixInfo object to be serialized</param>
    /// <returns>A strong containing formatted XML</returns>
    public static string SerializeAffixInfo(AffixInfo info)
    {
        MemoryStream stream = new MemoryStream();
        DataContractSerializer serializer = new DataContractSerializer(typeof(AffixInfo));
        XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

        using (XmlWriter writer = XmlWriter.Create(stream, settings))
            serializer.WriteObject(writer, info);
        stream.Position = 0;
        StreamReader reader = new StreamReader(stream);
        string data = reader.ReadToEnd();
        reader.Close();
        stream.Close();
        return data;
    }

    /// <summary>
    /// Deserializes an Affix Info object from a string
    /// </summary>
    /// <param name="data">The XML string describing the AffixInfo object</param>
    /// <returns>The usable AffixInfo object</returns>
    public static AffixInfo DeserializeAffixInfo(string data)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        DataContractSerializer serializer = new DataContractSerializer(typeof(AffixInfo));
        AffixInfo info = serializer.ReadObject(stream) as AffixInfo;
        stream.Close();
        return info;
    }

    /// <summary>
    /// Saves info about a certain affix type to disk.
    /// </summary>
    public static void SaveAffixInfoToDisk(AffixInfo info)
    {
        string data = SerializeAffixInfo(info);

        FileStream file = new FileStream(GetPathFromAffixType(info.Type), FileMode.Create);
        StreamWriter writer = new StreamWriter(file);
        writer.Write(data);
        writer.Close();
        file.Close();
    }

    /// <summary>
    /// Loads info about a certain affix type at disk and automatically calls its constructor so that it is added to the AffixInfoDictionary.
    /// </summary>
    /// <returns>The AffixInfo object, or null if nothing was found.</returns>
    public static AffixInfo LoadAffixInfoFromDisk(AffixType type)
    {
        FileStream file;
        try
        {
            file = new FileStream(GetPathFromAffixType(type), FileMode.Open);
        }
        catch (FileLoadException e)
        {
            Debug.LogError(e.Message);
            return null;
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError(e.Message);
            return null;
        }

        StreamReader reader = new StreamReader(file);
        string data = reader.ReadToEnd();
        reader.Close();
        file.Close();
        // Create new AffixInfo object in order to call its constructor which takes care of setup work
        return new AffixInfo(DeserializeAffixInfo(data));
    }

    private static string GetPathFromAffixType(AffixType type)
    {
        return AFFIX_INFO_FOLDER_PATH + type;
    }
    #endregion

    #region AffixPool
    public static void SaveAffixPoolsToDisk(Dictionary<AffixPoolPreset, AffixPool> pools)
    {
        FileStream file = new FileStream(AFFIX_POOL_FILE_PATH, FileMode.Create);

        DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<AffixPoolPreset, AffixPool>));
        XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

        using (XmlWriter writer = XmlWriter.Create(file, settings))
            serializer.WriteObject(writer, pools);
        file.Close();
    }

    public static Dictionary<AffixPoolPreset, AffixPool> LoadAffixPoolsFromDisk()
    {
        FileStream file;
        try
        {
            file = new FileStream(AFFIX_POOL_FILE_PATH, FileMode.Open);
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError(e.Message);
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return null;
        }

        StreamReader reader = new StreamReader(file);
        string data = reader.ReadToEnd();

        // Remove all newlines and spaces between individual tags because apparently this serializer doesn't like them
        int index;
        while ((index = data.IndexOf(Environment.NewLine)) != -1)
        {
            data = data.Remove(index, 2);
        }
        while ((index = data.IndexOf("> ")) != -1)
        {
            data = data.Remove(index + 1, 1);
        }

        reader.Close();
        file.Close();

        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data));

        Dictionary<AffixPoolPreset, AffixPool> pools = new Dictionary<AffixPoolPreset, AffixPool>();
        DataContractSerializer serializer = new DataContractSerializer(pools.GetType());
        pools = serializer.ReadObject(stream) as Dictionary<AffixPoolPreset, AffixPool>;
        stream.Close();

        // Create new random objects
        foreach (var pool in pools)
        {
            pool.Value.ChangeSeed();
        }

        return pools;
    }
    #endregion

    #region BiomeInfo
    public static string SerializeBiomeInfo(BiomeInfo info)
    {
        MemoryStream stream = new MemoryStream();
        DataContractSerializer serializer = new DataContractSerializer(typeof(BiomeInfo));
        XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

        using (XmlWriter writer = XmlWriter.Create(stream, settings))
            serializer.WriteObject(writer, info);

        stream.Position = 0;
        StreamReader reader = new StreamReader(stream);
        string data = reader.ReadToEnd();
        reader.Close();
        stream.Close();
        return data;
    }

    /// <summary>
    /// deserializes a biome info
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static BiomeInfo DeserializeBiomeInfo(string data)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        DataContractSerializer serializer = new DataContractSerializer(typeof(BiomeInfo));
        BiomeInfo info = serializer.ReadObject(stream) as BiomeInfo;
        stream.Close();
        return info;
    }

    /// <summary>
    /// Save Biome Info to disk
    /// </summary>
    public static void SaveBiomeInfoToDisk(BiomeInfo info)
    {
        string data = SerializeBiomeInfo(info);

        Debug.Log(data);

        FileStream file = new FileStream(GetBiomePathFromType(info.Type), FileMode.Create);
        StreamWriter writer = new StreamWriter(file);
        writer.Write(data);
        writer.Close();
        file.Close();
    }

    /// <summary>
    /// Loads the information about a biome from disk
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static BiomeInfo LoadBiomeInfoFromDisk(BiomeType type)
    {
        FileStream file;
        try
        {
            file = new FileStream(GetBiomePathFromType(type), FileMode.Open);
        }
        catch (FileLoadException e)
        {
            Debug.LogError(e.Message);
            return null;
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError(e.Message);
            return null;
        }

        StreamReader reader = new StreamReader(file);
        string data = reader.ReadToEnd();
        reader.Close();
        file.Close();
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
}

