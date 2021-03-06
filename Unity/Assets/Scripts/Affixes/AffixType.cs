﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

/// <summary>
/// Represents a single affix type, encoded by a string
/// </summary>
public sealed class AffixType : IEquatable<AffixType>
{
    #region Static
    private static int nextID = 0;
    // Some dicionaries to keep track of existing types
    private static Dictionary<int, AffixType> idDict = new Dictionary<int, AffixType>();
    private static Dictionary<string, AffixType> nameDict = new Dictionary<string, AffixType>();

    public static HashSet<AffixType> Types { get; private set; } = new HashSet<AffixType>();

    public static AffixType Get(int id)
    {
        if (!idDict.ContainsKey(id))
            throw new ArgumentException($"No affix type with ID {id}!");

        return idDict[id];
    }

    public static AffixType GetByName(string name)
    {
        if (!nameDict.ContainsKey(name))
            throw new ArgumentException($"No affix type with name '{name}'");

        return nameDict[name];
    }

    /// <summary>
    /// Creates a new affix type with the specified name
    /// </summary>
    /// <param name="name"></param>
    /// <returns>A reference to the new affix type</returns>
    public static AffixType CreateNew(string name)
    {
        return new AffixType(nextID++, name);
    }

    /// <summary>
    /// Replaces any existing affix types with either the provided ID or name with a new one
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static AffixType Replace(int id, string name)
    {
        if (idDict.ContainsKey(id))
        {
            var oldType = idDict[id];
            idDict.Remove(oldType.ID);
            nameDict.Remove(oldType.Name);
            Types.Remove(oldType);
        }
        if (nameDict.ContainsKey(name))
        {
            var oldType = nameDict[name];
            idDict.Remove(oldType.ID);
            nameDict.Remove(oldType.Name);
            Types.Remove(oldType);
        }

        return new AffixType(id, name);
    }

    /// <summary>
    /// Removes an existing type
    /// </summary>
    /// <param name="type"></param>
    public static void Remove(AffixType type)
    {
        idDict.Remove(type.ID);
        nameDict.Remove(type.Name);
        Types.Remove(type);
    }

    /// <summary>
    /// Check whether an affix type with a given ID exists
    /// </summary>
    public static bool Exists(int id)
    {
        return idDict.ContainsKey(id);
    }

    /// <summary>
    /// Check whether an affix type with a given name exists
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool Exists(string name)
    {
        return nameDict.ContainsKey(name);
    }

    /// <summary>
    /// Checks whether the name would be a valid affix name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool IsValidName(string name)
    {
        return !string.IsNullOrWhiteSpace(name);
    }

    #region Operators
    public static bool operator ==(AffixType lhs, AffixType rhs)
    {
        if (ReferenceEquals(lhs, rhs))
            return true;
        if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            return false;

        return lhs.ID == rhs.ID;
    }

    public static bool operator !=(AffixType lhs, AffixType rhs)
    {
        return !(lhs == rhs);
    }

    public static implicit operator AffixType(int value)
    {
        if (!idDict.ContainsKey(value))
            throw new ArgumentException($"Cannot convert {value} to AffixType as there is no type with that ID!");
        return idDict[value];
    }
    #endregion

    #region Predefined Values
    // Except for random and none, these shouldn't actually be used outside of testing
    public static AffixType Random = new AffixType(-2, "Random");
    public static AffixType None = new AffixType(-1, "None");

    public static int Health = 0;
    public static int PhysDmgFlat = 1;
    public static int FireRate = 2;
    #endregion

    #endregion

    [JsonIgnore]
    public int ID { get; private set; }
    public string Name { get; private set; }

    private AffixType(int id, string name)
    {
        if (idDict.ContainsKey(id))
            throw new ArgumentException($"An affix type with ID {id} already exists! Existing name: {Name}, new name: {name}");
        if (nameDict.ContainsKey(name))
            throw new ArgumentException($"An affix type with name '{name}' already exists! Existing ID: {nameDict[name].ID}, new ID: {id}");

        if (nextID <= id)
            nextID = id + 1;

        ID = id;
        Name = name;

        idDict[id] = this;
        nameDict[name] = this;

        if (ID >= 0)
            Types.Add(this);
    }

    /// <summary>
    /// Initializes this affix type to an existing type with the same name or creates a new one if it doesn't exist yet
    /// </summary>  
    [JsonConstructor]
    private AffixType(string name)
    {
        if (!nameDict.ContainsKey(name))
            ID = CreateNew(name).ID;
        else
            ID = nameDict[name].ID;

        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object obj)
    {
        if (obj is AffixType)
            return (obj as AffixType) == this;
        return base.Equals(obj);
    }

    public bool Equals(AffixType type)
    {
        return this == type;
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
}
