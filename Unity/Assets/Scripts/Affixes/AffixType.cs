using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;

// TODO: Improve design so that dynamically adding types is a thing but also no confusion with strings
/// <summary>
/// Represents a single affix type, encoded by a string
/// </summary>
[DataContract]
public sealed class AffixType
{
    #region Static
    private static int nextID = 0;
    // Some dicionaries to keep track of existing types
    private static Dictionary<int, string> names = new Dictionary<int, string>();
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

    public static bool operator ==(AffixType lhs, AffixType rhs)
    {
        return lhs.ID == rhs.ID;
    }

    public static bool operator !=(AffixType lhs, AffixType rhs)
    {
        return !(lhs == rhs);
    }
    #region Predefined Values
    // Except for random and none, these shouldn't actually be used outside of testing
    public static AffixType Random = new AffixType(-2, "Random");
    public static AffixType None = new AffixType(-1, "None");

    public static AffixType FireRate = new AffixType(0, "Fire Rate");
    public static AffixType Health = new AffixType(1, "Health");
    public static AffixType PhysDmgFlat = new AffixType(2, "Flat Physical Damage");
    #endregion
    #endregion

    [DataMember]
    public int ID { get; private set; }

    private AffixType(int id, string name)
    {
        if (idDict.ContainsKey(id))
            throw new ArgumentException($"An affix type with ID {id} already exists! Existing name: {names[id]}, new name: {name}");
        if (nameDict.ContainsKey(name))
            throw new ArgumentException($"An affix type with name '{name}' already exists! Existing ID: {nameDict[name].ID}, new ID: {id}");

        if (nextID <= id)
            nextID = id + 1;

        ID = id;
        names[id] = name;

        idDict[id] = this;
        nameDict[name] = this;

        if (ID >= 0)
            Types.Add(this);
    }

    private AffixType(string name) : this(nextID++, name)
    { }

    public override string ToString()
    {
        return names[ID];
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
