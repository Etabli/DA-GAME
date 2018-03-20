using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AffixValueInfoInput : MonoBehaviour
{
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
}
