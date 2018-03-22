using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

public abstract class AffixValueInfoInput : MonoBehaviour
{
    #region Static
    protected static List<string> progressionFunctions;

    protected void InitializeProgressionFunctions()
    {
        if (progressionFunctions == null)
        {
            progressionFunctions = typeof(AffixProgression).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Select(method => method.Name).ToList()
                .Where(name => !name.StartsWith("op")).ToList();
        }
    }
    #endregion

    public event Action<bool> OnChangedStatusUpdated;

    public abstract AffixValueInfo Info { get; }

    private bool _isChanged;
    public bool IsChanged
    {
        get { return _isChanged; }
        set
        {
            if (_isChanged != value)
            {
                _isChanged = value;
                OnChangedStatusUpdated?.Invoke(_isChanged);
            }
        }
    }

    public abstract bool IsValid { get; }

    public abstract void SetValueInfo(AffixValueInfo info);

    public abstract void Initialize();
}
