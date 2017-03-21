﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Value types for attributes. Used to decide on the proper AttributeValue child class upon deserialization.
/// </summary>
public enum AttributeValueType
{
    SingleValue,
    Range,
    Multiple
}

#region AttributeValue
/// <summary>
/// Represents a general AttributeValue and the operations it should support.
/// </summary>
public abstract class AttributeValue
{
    public abstract AttributeValue Invert();
    public abstract AttributeValue Add(float val);
    public abstract AttributeValue Add(AttributeValue val);
    public abstract AttributeValue Multiply(float val);

    public static AttributeValue operator+(AttributeValue lhs, float rhs)
    {
        return lhs.Add(rhs);
    }

    public static AttributeValue operator+(AttributeValue lhs, AttributeValue rhs)
    {
        return lhs.Add(rhs);
    }

    public static AttributeValue operator -(AttributeValue val)
    {
        return val.Invert();
    }

    public static AttributeValue operator-(AttributeValue lhs, float rhs)
    {
        return lhs + (-rhs);
    }

    public static AttributeValue operator-(AttributeValue lhs, AttributeValue rhs)
    {
        return lhs + (-rhs);
    }

    public static AttributeValue operator*(AttributeValue lhs, float rhs)
    {
        return lhs.Multiply(rhs);
    }

    protected void ThrowIncompatipleArgumentTypeException(AttributeValue val)
    {
        throw new ArgumentException(string.Format("Cannot add object of type {0} to object of type {1}", val.GetType(), this.GetType()), "val");
    }
}
#endregion

#region AttributeValueSingle
/// <summary>
/// An AttributeValue that represents a single float.
/// </summary>
public class AttributeValueSingle : AttributeValue
{
    public float Value { get; protected set; }

    public AttributeValueSingle()
    {}

    public AttributeValueSingle(float value)
    {
        Value = value;
    }

    #region Operators
    public override AttributeValue Invert()
    {
        return new AttributeValueSingle(-Value);
    }

    public override AttributeValue Add(float val)
    {
        return new AttributeValueSingle() { Value = Value + val };
    }

    public override AttributeValue Add(AttributeValue val)
    {
        if (val is AttributeValueSingle)
        {
            return Add(((AttributeValueSingle)val).Value);
        }
        ThrowIncompatipleArgumentTypeException(val);
        return null;
    }

    public override AttributeValue Multiply(float val)
    {
        return new AttributeValueSingle() { Value = Value * val };
    }
    #endregion

    public override string ToString()
    {
        return Value.ToString();
    }
}
#endregion

#region AttributeValueRange
/// <summary>
/// An AttributeValue that represents a Range.
/// </summary>
public class AttributeValueRange : AttributeValue
{
    public Range Value { get; protected set; }

    public AttributeValueRange()
    {}

    public AttributeValueRange(float min, float max) : this(new Range(min, max))
    { }

    public AttributeValueRange(Range value)
    {
        Value = value;
    }

    #region Operators
    public override AttributeValue Invert()
    {
        return new AttributeValueRange(-Value);
    }

    public override AttributeValue Add(float val)
    {
        return new AttributeValueRange() { Value = Value + val };
    }

    public override AttributeValue Multiply(float val)
    {
        return new AttributeValueRange() { Value = Value * val };
    }

    public override AttributeValue Add(AttributeValue val)
    {
        if (val is AttributeValueRange)
        {
            return this + ((AttributeValueRange)val).Value;
        }
        else if (val is AttributeValueSingle)
        {
            return Add(((AttributeValueSingle)val).Value);
        }
        ThrowIncompatipleArgumentTypeException(val);
        return null;
    }

    public static AttributeValueRange operator+(AttributeValueRange lhs, Range rhs)
    {
        return new AttributeValueRange() { Value = lhs.Value + rhs };
    }
    #endregion

    public override string ToString()
    {
        return Value.ToString();
    }
}
#endregion

#region AttributeValueMultiple
/// <summary>
/// An AttributeValue that represents an array of floats.
/// </summary>
public class AttributeValueMultiple : AttributeValue
{
    public float[] Values { get; protected set; }

    public AttributeValueMultiple()
    { }

    public AttributeValueMultiple(params float[] values)
    {
        Values = new float[values.Length];
        values.CopyTo(Values, 0);
    }

    #region Operators
    public override AttributeValue Invert()
    {
        return new AttributeValueMultiple() { Values = Values.Select(v => -v).ToArray() };
    }

    public override AttributeValue Add(float val)
    {
        return new AttributeValueMultiple() { Values = Values.Select(v => v + val).ToArray() };
    }

    public override AttributeValue Add(AttributeValue val)
    {
        if (val is AttributeValueMultiple)
        {
            AttributeValueMultiple valMultiple = val as AttributeValueMultiple;
            float[] vals = new float[Values.Length];
            Values.CopyTo(vals, 0);
            for (int i = 0; i < Values.Length; i++)
            {
                vals[i] += valMultiple.Values[i];
            }
        }
        else if (val is AttributeValueSingle)
        {
            return Add(((AttributeValueSingle)val).Value);
        }
        ThrowIncompatipleArgumentTypeException(val);
        return null;
    }

    public override AttributeValue Multiply(float val)
    {
        return new AttributeValueMultiple() { Values = Values.Select(v => v * val).ToArray() };
    }
    #endregion

    public override string ToString()
    {
        string result = "";
        foreach(float v in Values)
        {
            result += v + ", ";
        }
        result.TrimEnd(',', ' ');
        return result;
    }
}
#endregion
