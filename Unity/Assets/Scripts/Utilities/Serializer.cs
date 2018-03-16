using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using UnityEngine;

public abstract class Serializer
{
    const string DATA_FOLDER_PATH = "Data/";
    const string AFFIX_FOLDER_PATH = DATA_FOLDER_PATH + "Affix/";
    const string AFFIX_INFO_FOLDER_PATH = AFFIX_FOLDER_PATH + "AffixInfo/";
    const string AFFIX_POOL_FILE_PATH = AFFIX_FOLDER_PATH + "AffixPools";
    const string AFFIX_TYPE_FILE_PATH = AFFIX_FOLDER_PATH + "AffixTypes";
    const string BIOME_FOLDER_PATH = DATA_FOLDER_PATH + "Biome/";

    #region AffixInfo
    /// <summary>
    /// Takes an AffixInfo object and returns a string containing formatted XML describing it.
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
    /// Deserializes an Affix Info object from a string. Automatically calls its constructor to perform setup work.
    /// </summary>
    /// <param name="data">The XML string describing the AffixInfo object</param>
    /// <returns>The usable AffixInfo object</returns>
    public static AffixInfo DeserializeAffixInfo(string data)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        DataContractSerializer serializer = new DataContractSerializer(typeof(AffixInfo));
        AffixInfo info = serializer.ReadObject(stream) as AffixInfo;
        stream.Close();
        return new AffixInfo(info);
    }

    /// <summary>
    /// Saves info about a certain affix type to disk.
    /// </summary>
    public static void SaveAffixInfoToDisk(AffixInfo info)
    {
        SaveAffixInfoToDisk(info, GetPathFromAffixType(info.Type));
    }

    /// <summary>
    /// Saves an AffixInfo to disk at a specified location.
    /// </summary>
    /// <param name="path">The full path, including filename</param>
    public static void SaveAffixInfoToDisk(AffixInfo info, string path)
    {
        string data = SerializeAffixInfo(info);

        FileStream file = new FileStream(path, FileMode.Create);
        StreamWriter writer = new StreamWriter(file);
        writer.Write(data);
        writer.Close();
        file.Close();
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

        string data;
        using (MemoryStream stream = new MemoryStream(text.bytes))
            using (StreamReader reader = new StreamReader(stream))
                data = reader.ReadToEnd();

        return DeserializeAffixInfo(data);
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
    /// <param name="folder"></param>
    public static void LoadAllAffixInfosFromDisk(string folder)
    {
        try
        {
            foreach (AffixType type in AffixType.Types)
            {
                AffixInfo.Register(LoadAffixInfoFromDisk(folder + GetFileNameFromAffixType(type)));
            }
        }
        catch (FileNotFoundException e)
        {
            throw new ArgumentException($"{folder} is not a valid affix info folder path!", nameof(folder), e);
        }
        catch (SerializationException e)
        {
            throw new SerializationException($"Encountered invalid affix info file while loading all from '{folder}'!", e);
        }
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
        FileStream file = new FileStream(AFFIX_POOL_FILE_PATH, FileMode.Create);

        DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<AffixPoolPreset, AffixPool>));
        XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

        using (XmlWriter writer = XmlWriter.Create(file, settings))
            serializer.WriteObject(writer, pools);
        file.Close();
    }

    public static Dictionary<AffixPoolPreset, AffixPool> LoadAffixPoolsFromDisk()
    {
        TextAsset text = Resources.Load<TextAsset>(AFFIX_POOL_FILE_PATH);
        if (text == null)
            throw new ArgumentException($"{AFFIX_POOL_FILE_PATH} is not a valid text asset!");

        string data;
        using (MemoryStream stream = new MemoryStream(text.bytes))
            using (StreamReader reader = new StreamReader(stream))
                data = reader.ReadToEnd();

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

        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
        {
            Dictionary<AffixPoolPreset, AffixPool> pools = new Dictionary<AffixPoolPreset, AffixPool>();
            DataContractSerializer serializer = new DataContractSerializer(pools.GetType());
            pools = serializer.ReadObject(stream) as Dictionary<AffixPoolPreset, AffixPool>;

            // Create new random objects
            foreach (var pool in pools)
            {
                pool.Value.ChangeSeed();
            }

            return pools;
        }
    }
    #endregion

    #region AffixType
    public static void SaveAffixTypesToDisk(IEnumerable<AffixType> types)
    {
        using (FileStream file = new FileStream(AFFIX_TYPE_FILE_PATH, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(file))
            {
                foreach (string type in types)
                {
                    writer.WriteLine(type);
                }
            }
        }
    }

    public static HashSet<AffixType> LoadAffixTypesFromDisk()
    {
        TextAsset text = Resources.Load<TextAsset>(AFFIX_TYPE_FILE_PATH);
        if (text == null)
            throw new FileNotFoundException("Couldn't find affix type info file!");

        HashSet<AffixType> result = new HashSet<AffixType>();
        using (MemoryStream stream = new MemoryStream(text.bytes))
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    result.Add(line);
            }
        }

        return result;
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

        //Debug.Log(data);

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
}

