using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.IO;
using System.Text;

public class AttributeInfoSerializer
{
    public static string SerializeAttributeInfo(AttributeInfo info)
    {
        MemoryStream stream = new MemoryStream();
        DataContractSerializer serializer = new DataContractSerializer(typeof(AttributeInfo));
        serializer.WriteObject(stream, info);
        stream.Position = 0;
        StreamReader reader = new StreamReader(stream);
        string data = reader.ReadToEnd();
        reader.Close();
        stream.Close();
        return data;
    }

    public static AttributeInfo DeserializeAttributeInfo(string data)
    { 
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        DataContractSerializer serializer = new DataContractSerializer(typeof(AttributeInfo));
        AttributeInfo info = serializer.ReadObject(stream) as AttributeInfo;
        stream.Close();
        return info;
    }
}
