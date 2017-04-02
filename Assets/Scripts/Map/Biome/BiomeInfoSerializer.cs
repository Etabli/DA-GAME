using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;

/// <summary>
/// Maybe a generic serializer would be best
/// but that's a job for anotehr time
/// This feelts so dirty
/// </summary>
public static class BiomeInfoSerializer {

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

        FileStream file = new FileStream(GetPathFromType(info.Type), FileMode.Create);
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
            file = new FileStream(GetPathFromType(type), FileMode.Open);
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
        foreach(BiomeType type in System.Enum.GetValues(typeof(BiomeType)))
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
    private static string GetPathFromType(BiomeType type)
    {
        return "Assets\\Data\\Biome\\" + type;
    }
}
