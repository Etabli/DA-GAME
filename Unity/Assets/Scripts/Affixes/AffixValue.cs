﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

#region AffixValue
/// <summary>
/// Represents a general AffixValue and the operations it should support.
/// </summary>
public abstract class AffixValue
{
    // Not an interface because interfaces can't contain operators
    #region Operators
    public abstract AffixValue Invert();
    public abstract AffixValue Add(float val);
    public abstract AffixValue Add(AffixValue val);
    public abstract AffixValue Multiply(float val);

    public static AffixValue operator+(AffixValue lhs, float rhs)
    {
        return lhs.Add(rhs);
    }

    public static AffixValue operator +(float lhs, AffixValue rhs)
    {
        return rhs + lhs;
    }

    public static AffixValue operator+(AffixValue lhs, AffixValue rhs)
    {
        return lhs.Add(rhs);
    }

    public static AffixValue operator -(AffixValue val)
    {
        return val.Invert();
    }

    public static AffixValue operator-(AffixValue lhs, float rhs)
    {
        return lhs + (-rhs);
    }

    public static AffixValue operator-(AffixValue lhs, AffixValue rhs)
    {
        return lhs + (-rhs);
    }

    public static AffixValue operator*(AffixValue lhs, float rhs)
    {
        return lhs.Multiply(rhs);
    }

    public static AffixValue operator*(float lhs, AffixValue rhs)
    {
        return rhs.Multiply(lhs);
    }

    public static bool operator==(AffixValue lhs, AffixValue rhs)
    {
        if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            return false;

        if (lhs.IsSameType(rhs))
        {
            if (lhs is AffixValueSingle)
                return (lhs as AffixValueSingle) == (rhs as AffixValueSingle);
            else if (lhs is AffixValueRange)
                return (lhs as AffixValueRange) == (rhs as AffixValueRange);
            else
                return (lhs as AffixValueMultiple) == (rhs as AffixValueMultiple);
        }
        return false;
    }

    public static bool operator!=(AffixValue lhs, AffixValue rhs)
    {
        return !(lhs == rhs);
    }

    public override bool Equals(object obj)
    {
        if (obj is AffixValue)
            return (obj as AffixValue) == this;
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    protected void ThrowIncompatipleArgumentTypeException(AffixValue val)
    {
        throw new ArgumentException(string.Format("Cannot add object of type {0} to object of type {1}", val.GetType(), this.GetType()), "val");
    }
    #endregion

    #region Conversions
    public static implicit operator AffixValue(float value)
    {
        return new AffixValueSingle(value);
    }
    #endregion

    #region Type Comparison
    public abstract bool IsSameType(AffixValue affixValue);
    #endregion
}
#endregion

#region AffixValueSingle
/// <summary>
/// An AffixValue that represents a single float.
/// </summary>
public class AffixValueSingle : AffixValue
{
    public float Value { get; protected set; }

    public AffixValueSingle() : this(0f)
    {}

    [JsonConstructor]
    public AffixValueSingle(float value)
    {
        Value = value;
    }

    #region Comparison
    public override bool IsSameType(AffixValue affixValue)
    {
        return affixValue is AffixValueSingle;
    }

    public override bool Equals(object obj)
    {
        if (obj is AffixValueSingle)
            return this == (AffixValueSingle)obj;
        if (obj is float)
            return this == (float)obj;
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(AffixValueSingle lhs, AffixValueSingle rhs)
    {
        if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
            return true;
        if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            return false;

        return lhs.Value == rhs.Value;
    }

    public static bool operator !=(AffixValueSingle lhs, AffixValueSingle rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator ==(AffixValueSingle lhs, float rhs)
    {
        if (ReferenceEquals(lhs, null))
            return false;

        return lhs.Value == rhs;
    }

    public static bool operator !=(AffixValueSingle lhs, float rhs)
    {
        return lhs.Value != rhs;
    }

    public static bool operator ==(float lhs, AffixValueSingle rhs)
    {
        if (ReferenceEquals(rhs, null))
            return false;

        return rhs == lhs;
    }

    public static bool operator !=(float lhs, AffixValueSingle rhs)
    {
        return rhs != lhs;
    }
    #endregion

    #region Operators
    public override AffixValue Invert()
    {
        return new AffixValueSingle(-Value);
    }

    public override AffixValue Add(float val)
    {
        return new AffixValueSingle() { Value = Value + val };
    }

    public override AffixValue Add(AffixValue val)
    {
        if (val is AffixValueSingle)
        {
            return Add(((AffixValueSingle)val).Value);
        }
        ThrowIncompatipleArgumentTypeException(val);
        return null;
    }

    public override AffixValue Multiply(float val)
    {
        return new AffixValueSingle() { Value = Value * val };
    }
    #endregion

    #region Conversions
    public static implicit operator float(AffixValueSingle value)
    {
        return value.Value;
    }

    public static implicit operator AffixValueSingle(float value)
    {
        return new AffixValueSingle(value);
    }
    #endregion

    public override string ToString()
    {
        return Value.ToString();
    }
}
#endregion

#region AffixValueRange
/// <summary>
/// An AffixValue that represents a Range.
/// </summary>
public class AffixValueRange : AffixValue
{
    [JsonIgnore]
    public Range Value;

    public float MinValue
    {
        get { return Value.MinValue; }
        set { Value.MinValue = value; }
    }

    public float MaxValue
    {
        get { return Value.MaxValue; }
        set { Value.MaxValue = value; }

    }

    public AffixValueRange()
    {}

    [JsonConstructor]
    public AffixValueRange(float min, float max) : this(new Range(min, max))
    { }

    public AffixValueRange(Range value)
    {
        Value = value;
    }

    #region Comparison
    public override bool IsSameType(AffixValue affixValue)
    {
        return affixValue is AffixValueRange;
    }

    public override bool Equals(object obj)
    {
        if (obj is AffixValueRange)
            return this == obj as AffixValueRange;
        if (obj is Range)
            return this == (Range)obj;
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(AffixValueRange lhs, AffixValueRange rhs)
    {
        return lhs.Value == rhs.Value;
    }

    public static bool operator !=(AffixValueRange lhs, AffixValueRange rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator ==(AffixValueRange lhs, Range rhs)
    {
        return lhs.Value == rhs;
    }

    public static bool operator !=(AffixValueRange lhs, Range rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator ==(Range lhs, AffixValueRange rhs)
    {
        return rhs.Value == lhs;
    }

    public static bool operator !=(Range lhs, AffixValueRange rhs)
    {
        return !(rhs == lhs);
    }
    #endregion

    #region Arithmetic Operators
    public override AffixValue Invert()
    {
        return new AffixValueRange(-Value);
    }

    public override AffixValue Add(float val)
    {
        return new AffixValueRange() { Value = Value + val };
    }

    public override AffixValue Multiply(float val)
    {
        return new AffixValueRange() { Value = Value * val };
    }

    public override AffixValue Add(AffixValue val)
    {
        if (val is AffixValueRange)
        {
            return this + ((AffixValueRange)val).Value;
        }
        else if (val is AffixValueSingle)
        {
            return Add(((AffixValueSingle)val).Value);
        }
        ThrowIncompatipleArgumentTypeException(val);
        return null;
    }

    public static AffixValueRange operator+(AffixValueRange lhs, Range rhs)
    {
        return new AffixValueRange() { Value = lhs.Value + rhs };
    }
    #endregion

    #region Converions
    public static implicit operator Range(AffixValueRange value)
    {
        return value.Value;
    }

    public static implicit operator AffixValueRange(Range value)
    {
        return new AffixValueRange(value);
    }
    #endregion

    public override string ToString()
    {
        return Value.ToString();
    }
}
#endregion

#region AffixValueMultiple
/// <summary>
/// An AffixValue that represents an array of floats.
/// </summary>
public class AffixValueMultiple : AffixValue
{
    public AffixValue[] Values { get; protected set; }
    [JsonIgnore]
    protected readonly Dictionary<string, int> valueNames = new Dictionary<string, int>();

    public int Count { get { return Values.Length; } }

    public AffixValueMultiple()
    { }

    [JsonConstructor]
    public AffixValueMultiple(params AffixValue[] values)
    {
        Values = new AffixValue[values.Length];
        values.CopyTo(Values, 0);
    }

    public AffixValueMultiple(Tuple<string, AffixValue>[] values)
    {
        Values = new AffixValue[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            var tuple = values[i];
            Values[i] = tuple.Item2;
            valueNames.Add(tuple.Item1, i);
        }
    }

    public AffixValueMultiple(AffixValue[] values, string[] names)
        : this(values)
    {
        if (values.Length != names.Length)
            throw new ArgumentException("Value and name arrays have to have same length!");

        for (int i = 0; i < values.Length; i++)
        {
            if (valueNames.ContainsKey(names[i]))
                throw new ArgumentException("Name array must not contain duplicates!", nameof(names));
            valueNames.Add(names[i], i);
        }
    }

    public AffixValue this[int index]
    {
        get
        {
            if (index < 0 || index >= Values.Length)
                throw new IndexOutOfRangeException();

            return Values[index];
        }
    }

    public AffixValue this[string name]
    {
        get
        {
            if (!valueNames.ContainsKey(name))
                throw new KeyNotFoundException($"No value with name {name} exists!");

            return Values[valueNames[name]];
        }
    }

    #region Comparison
    public override bool IsSameType(AffixValue affixValue)
    {
        if (!(affixValue is AffixValueMultiple))
        {
            return false;
        }

        var affixValueMultiple = affixValue as AffixValueMultiple;
        if (affixValueMultiple.Values.Length != Values.Length)
            return false;

        for (int i = 0; i < Values.Length; i++)
        {
            if (!affixValueMultiple.Values[i].IsSameType(Values[i]))
                return false;
        }
        return true; ;
    }

    public override bool Equals(object obj)
    {
        if (obj is AffixValueMultiple)
            return this == (AffixValueMultiple)obj;
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(AffixValueMultiple lhs, AffixValueMultiple rhs)
    {
        return lhs.Values.SequenceEqual(rhs.Values);
    }

    public static bool operator !=(AffixValueMultiple lhs, AffixValueMultiple rhs)
    {
        return !(lhs == rhs);
    }
    #endregion

    #region Operators
    public override AffixValue Invert()
    {
        return new AffixValueMultiple() { Values = Values.Select(v => -v).ToArray() };
    }

    public override AffixValue Add(float val)
    {
        return new AffixValueMultiple() { Values = Values.Select(v => v + val).ToArray() };
    }

    public override AffixValue Add(AffixValue val)
    {
        if (val is AffixValueMultiple)
        {
            AffixValueMultiple valMultiple = val as AffixValueMultiple;
            AffixValue[] vals = new AffixValue[Values.Length];
            Values.CopyTo(vals, 0);
            for (int i = 0; i < Values.Length; i++)
            {
                vals[i] += valMultiple.Values[i];
            }
        }
        else if (val is AffixValueSingle)
        {
            return Add(((AffixValueSingle)val).Value);
        }
        ThrowIncompatipleArgumentTypeException(val);
        return null;
    }

    public override AffixValue Multiply(float val)
    {
        return new AffixValueMultiple() { Values = Values.Select(v => v * val).ToArray() };
    }
    #endregion

    public override string ToString()
    {
        string result = "";
        foreach(var v in Values)
        {
            result += v + ", ";
        }
        result.TrimEnd(',', ' ');
        return result;
    }
}
#endregion
