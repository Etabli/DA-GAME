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
    const string ATTRIBUTE_POOL_FILE_PATH = DATA_FOLDER_PATH + "AttributePools";
    const string BIOME_FOLDER_PATH = DATA_FOLDER_PATH + "Biome\\";

    #region AttributeInfo
    /// <summary>
    /// Takes an AttributeInfo object and returns a string containing formatted XML describing it
    /// </summary>
    /// <param name="info">The AttributeInfo object to be serialized</param>
    /// <returns>A strong containing formatted XML</returns>
    public static string SerializeAttributeInfo(AttributeInfo info)
    {
        MemoryStream stream = new MemoryStream();
        DataContractSerializer serializer = new DataContractSerializer(typeof(AttributeInfo));
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
    /// Deserializes an AttributeInfo object from a string
    /// </summary>
    /// <param name="data">The XML string describing the AttributeInfo object</param>
    /// <returns>The usable AttributeInfo object</returns>
    public static AttributeInfo DeserializeAttributeInfo(string data)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        DataContractSerializer serializer = new DataContractSerializer(typeof(AttributeInfo));
        AttributeInfo info = serializer.ReadObject(stream) as AttributeInfo;
        stream.Close();
        return info;
    }

    /// <summary>
    /// Saves info about a certain attribute type to disk.
    /// </summary>
    public static void SaveAttributeInfoToDisk(AttributeInfo info)
    {
        string data = SerializeAttributeInfo(info);

        FileStream file = new FileStream(GetPathFromAttributeType(info.Type), FileMode.Create);
        StreamWriter writer = new StreamWriter(file);
        writer.Write(data);
        writer.Close();
        file.Close();
    }

    /// <summary>
    /// Loads info about a certain attribute type at disk and automatically calls its constructor so that it is added to the AttributeInfoDictionary.
    /// </summary>
    /// <returns>The AttributeInfo object, or null if nothing was found.</returns>
    public static AttributeInfo LoadAttributeInfoFromDisk(AttributeType type)
    {
        FileStream file;
        try
        {
            file = new FileStream(GetPathFromAttributeType(type), FileMode.Open);
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
        // Create new AttributeInfo object in order to call its constructor which takes care of setup work
        return new AttributeInfo(DeserializeAttributeInfo(data));
    }

    private static string GetPathFromAttributeType(AttributeType type)
    {
        return DATA_FOLDER_PATH + type;
    }
    #endregion

    #region AttributePool
    public static void SaveAttributePoolsToDisk(Dictionary<AttributePoolPreset, AttributePool> pools)
    {
        FileStream file = new FileStream(ATTRIBUTE_POOL_FILE_PATH, FileMode.Create);

        DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<AttributePoolPreset, AttributePool>));
        XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

        using (XmlWriter writer = XmlWriter.Create(file, settings))
            serializer.WriteObject(writer, pools);
        file.Close();
    }

    public static Dictionary<AttributePoolPreset, AttributePool> LoadAttributePoolsFromDisk()
    {
        FileStream file;
        try
        {
            file = new FileStream(ATTRIBUTE_POOL_FILE_PATH, FileMode.Open);
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

        Dictionary<AttributePoolPreset, AttributePool> pools = new Dictionary<AttributePoolPreset, AttributePool>();
        DataContractSerializer serializer = new DataContractSerializer(pools.GetType());
        pools = serializer.ReadObject(stream) as Dictionary<AttributePoolPreset, AttributePool>;
        stream.Close();

        Debug.Log(pools.Count);

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

