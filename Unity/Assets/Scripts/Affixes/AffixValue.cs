using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

#region AffixValue
/// <summary>
/// Represents a general AffixValue and the operations it should support.
/// </summary>
[DataContract]
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
[DataContract]
public class AffixValueSingle : AffixValue
{
    [DataMember]
    public float Value { get; protected set; }

    public AffixValueSingle()
    {}

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
        return lhs.Value == rhs.Value;
    }

    public static bool operator !=(AffixValueSingle lhs, AffixValueSingle rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator ==(AffixValueSingle lhs, float rhs)
    {
        return lhs.Value == rhs;
    }

    public static bool operator !=(AffixValueSingle lhs, float rhs)
    {
        return lhs.Value != rhs;
    }

    public static bool operator ==(float lhs, AffixValueSingle rhs)
    {
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
[DataContract]
public class AffixValueRange : AffixValue
{
    [DataMember]
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
            return this == (AffixValueRange)obj;
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
        return lhs == rhs.Value;
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
        return lhs.Value != rhs;
    }

    public static bool operator ==(Range lhs, AffixValueRange rhs)
    {
        return rhs == lhs;
    }

    public static bool operator !=(Range lhs, AffixValueRange rhs)
    {
        return rhs != lhs;
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
[DataContract]
public class AffixValueMultiple : AffixValue
{
    [DataMember]
    public AffixValue[] Values { get; protected set; }
    [DataMember]
    protected Dictionary<string, int> valueNames = new Dictionary<string, int>();

    public AffixValueMultiple()
    { }

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
