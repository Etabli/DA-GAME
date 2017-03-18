using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    None,
    Physical,
    Fire,
    Energy
};

/// <summary>
/// Represents a specific damage instance of a certain type
/// </summary>
public class Damage
{
    public DamageType Type { get; protected set; }
    public float Value { get; protected set; }

    #region Constructors
    /// <summary>
    /// Creates an empty Damage object
    /// </summary>
    public Damage()
    {
        Type = DamageType.None;
        Value = 0;
    }

    /// <summary>
    /// Creates a Damage object with a certain type and value
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public Damage(DamageType type, float value)
    {
        Type = type;
        Value = value;
    }

    /// <summary>
    /// Creates a Damage object by copying a different one
    /// </summary>
    /// <param name="src">The source to be copied from</param>
    public Damage(Damage src)
    {
        Type = src.Type;
        Value = src.Value;
    }
    #endregion


    #region Operators
    public static Damage operator-(Damage obj)
    {
        return new Damage(obj.Type, -obj.Value);
    }

    /// <summary>
    /// Adds two damage instances. Returns the left hand side if the types don't match.
    /// </summary>
    public static Damage operator+(Damage lhs, Damage rhs)
    {
        Damage result = new Damage(lhs);
        if (lhs.Type == rhs.Type)
        {
            result.Value += rhs.Value;
        }
        return result;
    }

    public static Damage operator+(Damage lhs, float rhs)
    {
        return new Damage(lhs.Type, lhs.Value + rhs);
    }
    #endregion
}
