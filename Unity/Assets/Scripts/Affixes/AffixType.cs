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
public struct AffixType
{

    private static HashSet<AffixType> _Types;
    public static HashSet<AffixType> Types
    {
        get
        {
            if (_Types == null)
            {
                LoadTypesSet();
            }
            return _Types;
        }
    }

    [DataMember]
    private readonly string type;

    public AffixType(string type)
    {
        this.type = type;
    }

    #region Comparison
    public override bool Equals(object obj)
    {
        if (obj is AffixType)
            return ((AffixType)obj).type == type;
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return type.GetHashCode();
    }
    #endregion

    #region Conversion
    public static implicit operator AffixType(string value)
    {
        return new AffixType(value);
    }

    public static implicit operator string(AffixType affixType)
    {
        return affixType.type;
    }
    #endregion

    private static void LoadTypesSet()
    {
        _Types = Serializer.LoadAffixTypesFromDisk();
    }

    public override string ToString()
    {
        return type;
    }

    // Defininitions for all the affix types
    // Note that these are only relevant for writing code.
    // At runtime only the contents of the affix type file matter
    #region Static Types
    public const string Random = "Random";
    public const string None = "None";

    public const string Health = "Health";
    public const string PhysDmgFlat = "PhysDmgFlat";
    public const string FireRate = "FireRate";    
    #endregion
}
