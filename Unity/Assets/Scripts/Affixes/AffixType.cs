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

    private static HashSet<string> _Types;
    public static HashSet<string> Types
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
        // TODO: Load these from a file instead
        _Types = new HashSet<string>();
        foreach (string type in (typeof(AffixType))
            .GetFields(BindingFlags.Static | BindingFlags.Public)
            .Select(fieldInfo => fieldInfo.GetValue(null))
            .Cast<string>()
            .Where(type => type != Random && type != None))
        {
            _Types.Add(type);
        }
    }

    public override string ToString()
    {
        return type;
    }

    // Defininitions for all the affix types
    // Note that these automatically get aggregated into the Types set at runtime
    #region Static Types
    public const string Random = "Random";
    public const string None = "None";

    public const string Health = "Health";
    public const string PhysDmgFlat = "PhysDmgFlat";
    public const string FireRate = "FireRate";    
    #endregion
}
