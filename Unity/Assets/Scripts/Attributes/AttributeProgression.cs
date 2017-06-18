using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Reflection;

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
    public static AttributeValue Linear(AttributeValue value, int tier, float[] parameters)
    {
        if (parameters.Length != 2)
        {
            Debug.LogError("Linear progression requires exactly 2 parameters!");
            return null;
        }
        return (parameters[0] + parameters[1] * tier) * value;
    }

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
