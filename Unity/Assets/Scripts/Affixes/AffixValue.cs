using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

/// <summary>
/// Value types for affixes. Used to decide on the proper AffixValue child class upon deserialization.
/// </summary>
public enum AffixValueType
{
    SingleValue,
    Range,
    Multiple
}

#region AffixValue
/// <summary>
/// Represents a general AffixValue and the operations it should support.
/// </summary>
[DataContract]
public abstract class AffixValue
{
    #region Operators
    public abstract AffixValue Invert();
    public abstract AffixValue Add(float val);
    public abstract AffixValue Add(AffixValue val);
    public abstract AffixValue Multiply(float val);
    
    public static implicit operator AffixValue(float value)
    {
        return new AffixValueSingle(value);
    }

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
    public Range Value { get; protected set; }

    public AffixValueRange()
    {}

    public AffixValueRange(float min, float max) : this(new Range(min, max))
    { }

    public AffixValueRange(Range value)
    {
        Value = value;
    }

    #region Comparison
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
    public float[] Values { get; protected set; }

    public AffixValueMultiple()
    { }

    public AffixValueMultiple(params float[] values)
    {
        Values = new float[values.Length];
        values.CopyTo(Values, 0);
    }

    #region Comparison
    public override bool Equals(object obj)
    {
        if (obj is AffixValueMultiple)
            return this == (AffixValueMultiple)obj;
        if (obj is float[])
            return this == (float[])obj;
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(AffixValueMultiple lhs, AffixValueMultiple rhs)
    {
        return lhs == rhs.Values;
    }

    public static bool operator !=(AffixValueMultiple lhs, AffixValueMultiple rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator ==(AffixValueMultiple lhs, float[] rhs)
    {
        return lhs.Values.SequenceEqual(rhs);
    }

    public static bool operator !=(AffixValueMultiple lhs, float[] rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator ==(float[] lhs, AffixValueMultiple rhs)
    {
        return rhs == lhs;
    }

    public static bool operator !=(float[] lhs, AffixValueMultiple rhs)
    {
        return rhs != lhs;
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
            float[] vals = new float[Values.Length];
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
        foreach(float v in Values)
        {
            result += v + ", ";
        }
        result.TrimEnd(',', ' ');
        return result;
    }
}
#endregion
