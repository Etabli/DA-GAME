using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AffixInfoDisplay : MonoBehaviour
{
    public event Action<bool> OnChangedStatusUpdated;

    public Text NameDisplay;
    public AffixDescriptionDisplay DescriptionDisplay;
    public AffixValueInfoDisplay ValueInfoDisplay;

    #region IsChanged Logic
    bool _isChanged = false;
    public bool IsChanged
    {
        get { return _isChanged; }
        private set
        {
            if (_isChanged != value)
            {
                _isChanged = value;
                OnChangedStatusUpdated?.Invoke(_isChanged);
            }
        }
    }

    bool _isDescriptionChanged;
    bool IsDescriptionChanged
    {
        get { return _isDescriptionChanged; }
        set
        {
            if (_isDescriptionChanged != value)
            {
                _isDescriptionChanged = value;
                UpdateIsChanged();
            }
        }
    }

    bool _isValueInfoChanged;
    bool IsValueInfoChanged
    {
        get { return _isValueInfoChanged; }
        set
        {
            if (_isValueInfoChanged != value)
            {
                _isValueInfoChanged = value;
                UpdateIsChanged();
            }
        }
    }

    void UpdateIsChanged()
    {
        IsChanged = IsValueInfoChanged || IsDescriptionChanged;
    }
    #endregion

    public bool IsValid
    {
        get { return ValueInfoDisplay.IsValid && DescriptionDisplay.IsValid; }
    }

    /// <summary>
    /// An affix info object containing all updated information. Returns null
    /// if any input is invalid.
    /// </summary>
    public AffixInfo Info
    {
        get
        {
            if (!IsValid)
                return null;

            return new AffixInfo(currentInfo.Type, ValueInfoDisplay.Info, DescriptionDisplay.Text);
        }
    }

    AffixInfo currentInfo;

	void Start ()
    {
        DescriptionDisplay.OnChangedStatusUpdated += isChanged => IsDescriptionChanged = isChanged;
        ValueInfoDisplay.OnChangedStatusUpdated += isChanged => IsValueInfoChanged = isChanged;
	}

    public void SetType(AffixType type)
    {
        currentInfo = AffixInfo.GetAffixInfo(type);
        NameDisplay.text = currentInfo.Name;
        DescriptionDisplay.SetText(currentInfo.Description);
        ValueInfoDisplay.SetInfo(currentInfo.ValueInfo);
    }
}
