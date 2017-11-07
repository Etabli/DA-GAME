using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.Xml;

public class AttributeInfoSerializer
{
    //TODO: Move Serialization of different objects into same class/file

    const string DATA_PATH = "Assets\\Data\\";

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
    public static void SaveToDisk(AttributeInfo info)
    {
        string data = SerializeAttributeInfo(info);

        FileStream file = new FileStream(GetPathFromType(info.Type), FileMode.Create);
        StreamWriter writer = new StreamWriter(file);
        writer.Write(data);
        writer.Close();
        file.Close();
    }

    /// <summary>
    /// Loads info about a certain attribute type at disk and automatically calls its constructor so that it is added to the AttributeInfoDictionary.
    /// </summary>
    /// <returns>The AttributeInfo object, or null if nothing was found.</returns>
    public static AttributeInfo LoadFromDisk(AttributeType type)
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
        // Create new AttributeInfo object in order to call its constructor which takes care of setup work
        return new AttributeInfo(DeserializeAttributeInfo(data));
    }

    private static string GetPathFromType(AttributeType type)
    {
        return DATA_PATH + type;
    }
}
