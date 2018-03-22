using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;

/// <summary>
/// Represents a single affix type, encoded by a string
/// </summary>
[DataContract]
public sealed class AffixType
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
        return new AffixType(name);
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
        }
        if (nameDict.ContainsKey(name))
        {
            var oldType = nameDict[name];
            idDict.Remove(oldType.ID);
            nameDict.Remove(oldType.Name);
        }

        return new AffixType(id, name);
    }

    #region Operators
    public static bool operator ==(AffixType lhs, AffixType rhs)
    {
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
    [DataMember]
    public int ID { get; private set; }

    [DataMember]
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

    private AffixType(string name) : this(nextID++, name)
    { }

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

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
}
