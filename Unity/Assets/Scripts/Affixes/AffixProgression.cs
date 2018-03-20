using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine;

/// <summary>
/// Represents a progression function that can be applied to an AffixValue
/// </summary>
[DataContract]
public struct AffixProgression
{
    [DataMember]
    public readonly float[] Parameters;
    [DataMember]
    public readonly string ProgressionFunctionName;

    private MethodInfo progressionFunction;

    public AffixProgression(AffixProgression src) : this(src.ProgressionFunctionName, src.Parameters)
    { }

    public AffixProgression(string name, params float[] parameters)
    { 
        Parameters = new float[parameters.Length];
        parameters.CopyTo(Parameters, 0);
        ProgressionFunctionName = name;
        progressionFunction = null;

        AttachProgressionFunction();
    }

    #region Comparison
    public override bool Equals(object obj)
    {
        if (obj is AffixProgression)
            return this == (AffixProgression)obj;
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator==(AffixProgression lhs, AffixProgression rhs)
    {
        if (lhs.ProgressionFunctionName != rhs.ProgressionFunctionName ||
            !lhs.Parameters.SequenceEqual(rhs.Parameters))
        {
            return false;
        }
        return true;
    }

    public static bool operator!=(AffixProgression lhs, AffixProgression rhs)
    {
        return !(lhs == rhs);
    }
    #endregion

    /// <summary>
    /// Tries to find a function of this class through name and store a reference to its MethodInfo
    /// in this object
    /// </summary>
    /// <returns>true if the function was found, false otherwise</returns>
    public void AttachProgressionFunction()
    {
        MethodInfo method = GetType().GetMethod(ProgressionFunctionName);

        if (method == null)
        {
            throw new InvalidProgressionFunctionException($"Could not find progression function with name {ProgressionFunctionName}!");
        }

        progressionFunction = method;
    }

    public bool HasProgressionFunction()
    {
        return progressionFunction != null;
    }

    public string GetName()
    {
        return ProgressionFunctionName;
    }

    /// <summary>
    /// Applies the progression function to an instance of AffixValue
    /// </summary>
    /// <param name="value">The value to apply the funciton to</param>
    /// <param name="tier">The tier of the affix</param>
    /// <returns>The progressed AffixValue</returns>
    public AffixValue Apply(AffixValue value, int tier)
    {
        if (progressionFunction == null)
        {
            throw new InvalidOperationException("Can't apply progression without a valid progression function set!");
        }

        return (AffixValue)progressionFunction.Invoke(null, new object[] { value, tier, Parameters });
    }

    public override string ToString()
    {
        return Parameters.Aggregate(ProgressionFunctionName, (output, value) => output + ", " + value.ToString());
    }

    #region Static Progression Functions
    /// <summary>
    /// Defines how many parameters each progression function expects
    /// </summary>
    public static Dictionary<string, int> ParameterRequirements = new Dictionary<string, int>
    {
        {nameof(Constant), 1},
        {nameof(Linear), 2},
        {nameof(Exponential), 1}
    };

    // Valid progression functions should follow the signature
    // public static Affixvalue <name>(AffixValue, int, float[])
    // and throw an exception if parameters are invalid

    /// <summary>
    /// Always returns the single parameter as result
    /// </summary>
    public static AffixValue Constant(AffixValue value, int tier, float[] parameters)
    {
        if (parameters.Length != ParameterRequirements[nameof(Constant)])
        {
            throw new ArgumentException("Constant progression requires exactly 1 parameter!", nameof(parameters));
        }
        return (value * 0) + parameters[0];
    }

    /// <summary>
    /// Simple linear progression of the form y = kx + d
    /// </summary>
    public static AffixValue Linear(AffixValue value, int tier, float[] parameters)
    {
        if (parameters.Length != ParameterRequirements[nameof(Linear)])
        {
            throw new ArgumentException("Linear progression requires exactly 2 parameters!", nameof(parameters));
        }
        return (parameters[0] + parameters[1] * tier) * value;
    }

    /// <summary>
    /// Exponential progression with the parameter as the base
    /// </summary>
    public static AffixValue Exponential(AffixValue value, int tier, float[] parameters)
    {
        if (parameters.Length != ParameterRequirements[nameof(Exponential)])
        {
            throw new ArgumentException("Exponential progression requires exactly 1 parameter!", nameof(parameters));
        }
        return Mathf.Pow(parameters[0], tier) * value;
    }
    #endregion

    #region Exceptions
    [System.Serializable]
    public class InvalidProgressionFunctionException : Exception
    {
        public InvalidProgressionFunctionException() { }
        public InvalidProgressionFunctionException(string message) : base(message) { }
        public InvalidProgressionFunctionException(string message, Exception inner) : base(message, inner) { }
        protected InvalidProgressionFunctionException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
    #endregion
}
