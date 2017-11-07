﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Reflection;

/// <summary>
/// Represents a progression function that can be applied to an AttributeValue
/// </summary>
[DataContract]
public class AttributeProgression
{
    [DataMember]
    public float[] Parameters;
    [DataMember]
    public string ProgressionFunctionName;

    protected MethodInfo progressionFunction;

    public AttributeProgression(string name, params float[] parameters)
    {
        Parameters = new float[parameters.Length];
        parameters.CopyTo(Parameters, 0);
        ProgressionFunctionName = name;

        AttachProgressionFunction();
    }

    /// <summary>
    /// Tries to find a function of this class through name and store a reference to its MethodInfo
    /// in this object
    /// </summary>
    /// <returns>true if the function was found, false otherwise</returns>
    public bool AttachProgressionFunction()
    {
        MethodInfo method = GetType().GetMethod(ProgressionFunctionName);
        if (method != null)
        {
            progressionFunction = method;
            //Debug.Log(string.Format("Succesfully found progression function '{0}'", ProgressionFunctionName));
            return true;
        }
        Debug.LogError(string.Format("Couldn't find progression function with name {0}", ProgressionFunctionName));
        return false;
    }

    public bool HasProgressionFunction()
    {
        return progressionFunction != null;
    }

    /// <summary>
    /// Applies the progression function to an instance of AttributeValue
    /// </summary>
    /// <param name="value">The value to apply the funciton to</param>
    /// <param name="tier">The tier of the attribute</param>
    /// <returns>The progressed AttributeValue</returns>
    public AttributeValue Apply(AttributeValue value, int tier)
    {
        if (progressionFunction == null)
        {
            Debug.LogError("No progression function set!");
            return null;
        }
        return (AttributeValue)progressionFunction.Invoke(null, new object[] { value, tier, Parameters });
    }

    #region Static Progression Functions
    // Valid progression functions should follow the signature
    // public static Attributevalue <name>(AttributeValue, int, float[])
    // and return null if the given parameters are invalid

    /// <summary>
    /// Simple linear progression of the form y = kx + d
    /// </summary>
    public static AttributeValue Linear(AttributeValue value, int tier, float[] parameters)
    {
        if (parameters.Length != 2)
        {
            Debug.LogError("Linear progression requires exactly 2 parameters!");
            return null;
        }
        return (parameters[0] + parameters[1] * tier) * value;
    }

    /// <summary>
    /// Always returns the single parameter as result
    /// </summary>
    public static AttributeValue Constant(AttributeValue value, int tier, float[] parameters)
    {
        if (parameters.Length != 1)
        {
            Debug.LogError("Constant progression requires exactly 1 parameter!");
            return null;
        }
        return (value * 0) + parameters[0];
    }
    #endregion
}
