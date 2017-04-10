using System.Collections;
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

    public void CombineInto(AttributePool pool)
    {
        // TODO: Maybe change so that odds stay even. Currently attributes in both pools have a higher chance to be drawn
        lottery.CombineInto(pool.lottery);
    }
    
    public AttributeType Evaluate()
    {
        return lottery.GetWinner();
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

    public static void SavePoolsToDisk()
    {
        FileStream file = new FileStream("Assets\\Data\\" + SAVE_FILE_NAME, FileMode.Create);     
        
        DataContractSerializer serializer = new DataContractSerializer(PresetPools.GetType());
        XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

        using (XmlWriter writer = XmlWriter.Create(file, settings))
            serializer.WriteObject(writer, PresetPools);
        file.Close();
    }

    public static void LoadPoolsFromDisk()
    {
        FileStream file;
        try
        {
            file = new FileStream("Assets\\Data\\" + SAVE_FILE_NAME, FileMode.Open);
        }
        catch (FileNotFoundException e)
        {
            Debug.LogError(e.Message);
            return;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return;
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

        Debug.Log(data);
        reader.Close();
        file.Close();

        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data));

        DataContractSerializer serializer = new DataContractSerializer(PresetPools.GetType());
        PresetPools = serializer.ReadObject(stream) as Dictionary<AttributePoolPreset, AttributePool>;
        stream.Close();
    }
    #endregion
}
