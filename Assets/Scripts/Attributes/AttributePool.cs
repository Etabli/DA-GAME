using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.IO;
using System;

[DataContract]
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
    private static Dictionary<Slot, AttributePool> SlotPools = new Dictionary<Slot, AttributePool>();

    public static void RegisterPoolForSlot(Slot slot, AttributePool pool)
    {
        if (SlotPools.ContainsKey(slot))
        {
            Debug.LogError(string.Format("AttributePool for slot {0} is already assigned!", slot));
            return;
        }
        SlotPools[slot] = pool;
    }

    public static void RegisterPoolForSlot(Slot slot, params AttributeType[] types)
    {
        RegisterPoolForSlot(slot, new AttributePool(types));
    }

    public static AttributePool GetPool(Slot slot)
    {
        if (SlotPools.ContainsKey(slot))
        {
            return SlotPools[slot];
        }
        Debug.LogError(string.Format("No AttributePool registered for slot {0}!", slot));
        return null;
    }

    public static AttributePool GetPool(params Slot[] slots)
    {
        AttributePool result = new AttributePool();
        foreach (Slot s in slots)
        {
            SlotPools[s].CombineInto(result);
        }
        return result;
    }

    public static void SavePoolsToDisk()
    {
        FileStream file = new FileStream("Assets\\Data\\SlotPools", FileMode.Create);
        DataContractSerializer serializer = new DataContractSerializer(SlotPools.GetType());
        serializer.WriteObject(file, SlotPools);
        file.Close();
    }

    public static void LoadPoolsFromDisk()
    {
        FileStream file;
        try
        {
            file = new FileStream("Assets\\Data\\SlotPools", FileMode.Open);
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
        DataContractSerializer serializer = new DataContractSerializer(SlotPools.GetType());
        SlotPools = serializer.ReadObject(file) as Dictionary<Slot, AttributePool>;
    }
    #endregion
}
