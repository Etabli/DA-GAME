using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

/// <summary>
/// Represents a range of values that can be randomly sampled from
/// </summary>
[DataContract]
public struct Range
{
    [DataMember]
    public float MinValue;
    [DataMember]
    public float MaxValue;

    #region Constructors
    /// <summary>
    /// Create a new Range
    /// </summary>
    /// <param name="min">The minimum value of the range</param>
    /// <param name="max">The maximum value of the range</param>
    public Range(float min, float max)
    {
        MinValue = min;
        MaxValue = max;
    }

    /// <summary>
    /// Copies another Range
    /// </summary>
    /// <param name="src">The Range to be copied from</param>
    public Range(Range src)
    {
        MinValue = src.MinValue;
        MaxValue = src.MaxValue;
    }
    #endregion

    /// <summary>
    /// Returns a random float between MinValue and MaxValue
    /// </summary>
    /// <returns>A random float between MinValue and MaxValue</returns>
    public float Evaluate()
    {
        return Random.Range(MinValue, MaxValue);
    }

    /// <summary>
    /// Combines this Range with another
    /// </summary>
    /// <param name="range">The Range to be combined with this one</param>
    public void Combine(Range range)
    {
        MinValue += range.MinValue;
        MaxValue += range.MaxValue; 
    }

    /// <summary>
    /// checks if a value is inside the minimum and maximum range
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool IsInRange(float value)
    {
        return (value >= MinValue) && (value <= MaxValue);
    }

    #region Operators
    /// <summary>
    /// Combines two Ranges and returns the result
    /// </summary>
    /// <param name="range1">The first Range</param>
    /// <param name="range2">The second Range</param>
    /// <returns>The combined Range</returns>
    public static Range Combine(Range range1, Range range2)
    {
        float min = range1.MinValue + range2.MinValue;
        float max = range1.MaxValue + range2.MaxValue;
        return new Range(min, max);
    }

    public static Range operator-(Range range)
    {
        return new Range(-range.MinValue, -range.MaxValue);
    }

    public static Range operator -(Range lhs, Range rhs)
    {
        return lhs + (-rhs);
    }

    public static Range operator+(Range lhs, Range rhs)
    {
        return Combine(lhs, rhs);
    }

    public static Range operator+(Range lhs, float rhs)
    {
        return new Range(lhs.MinValue + rhs, lhs.MaxValue + rhs);
    }

    public static Range operator*(Range lhs, float rhs)
    {
        return new Range(lhs.MinValue * rhs, lhs.MaxValue * rhs);
    }
    #endregion

    public override string ToString()
    {
        return string.Format("{0} to {1}", MinValue, MaxValue);
    }
}
