using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a range of possible damage values of a certain type
/// </summary>
public struct DamageRange
{
    public DamageType Type;

    private Range range;

    #region Constructors
    /// <summary>
    /// Creates a new DamageRange object
    /// </summary>
    /// <param name="range">The range of values to be used</param>
    /// <param name="type">The damage type</param>
    public DamageRange(Range range, DamageType type) : this(range.MinValue, range.MaxValue, type)
    {}

    /// <summary>
    /// Creates a new DamageRange object
    /// </summary>
    /// <param name="min">The minium damage value</param>
    /// <param name="max">The maximum damgae value</param>
    /// <param name="type">The damage type</param>
    public DamageRange(float min, float max, DamageType type)
    {
        Type = type;
        range = new Range(min, max);
    }

    /// <summary>
    /// Creates a new DamageRange object by copying a different one
    /// </summary>
    /// <param name="src">The source to be copied from</param>
    public DamageRange(DamageRange src)
    {
        range = src.range;
        Type = src.Type;
    }
    #endregion

    /// <summary>
    /// Returns a damage instance with a random value inside the range
    /// </summary>
    /// <returns>A damage instance with a random value inside the range</returns>
    public Damage Evaluate()
    {
        return new Damage(Type, range.Evaluate());
    }

    #region Operators
    public static DamageRange operator-(DamageRange obj)
    {
        DamageRange result = new DamageRange(obj);
        result.range = -result.range;
        return result;
    }

    /// <summary>
    /// Adds two damage ranges. Returns the left hand side value if the damage types don't match.
    /// </summary>
    public static DamageRange operator+(DamageRange lhs, DamageRange rhs)
    {
        DamageRange result = new DamageRange(lhs);

        if (lhs.Type == rhs.Type)
        {
            lhs.range += rhs.range;
        }

        return result;
    }

    public static DamageRange operator+(DamageRange lhs, float rhs)
    {
        DamageRange result = new DamageRange(lhs);
        result.range += rhs;
        return result;
    }
    #endregion
}
