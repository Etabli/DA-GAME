using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AffixValueInfoSingleInput : AffixValueInfoInput
{
    public InputField MinimumInput;
    public InputField MaximumInput;

    #region IsChanged Logic
    private bool _isMinimumChanged;
    private bool IsMinimumChanged
    {
        get { return _isMinimumChanged; }
        set
        {
            if (_isMinimumChanged != value)
            {
                _isMinimumChanged = value;
                UpdateIsChanged();
            }
        }
    }

    private bool _isMaximumChanged;
    private bool IsMaximumChanged
    {
        get { return _isMaximumChanged; }
        set
        {
            if (_isMaximumChanged != value)
            {
                _isMaximumChanged = value;
                UpdateIsChanged();
            }
        }
    }
    #endregion

    public override AffixValueInfo Info
    {
        get
        {
            if (IsValid)
                return new AffixValueInfo(float.Parse(MinimumInput.text), float.Parse(MaximumInput.text));
            return null;
        }
    }

    public override bool IsValid
    {
        get
        {
            return isMinimumValid && isMaximumValid;
        }
    }

    public float Minimum
    {
        get
        {
            if (!isMinimumValid)
                return 0f;
            return float.Parse(MinimumInput.text);
        }
    }

    public float Maximum
    {
        get
        {
            if (!isMaximumValid)
                return 0f;
            return float.Parse(MaximumInput.text);
        }
    }

    private bool isMinimumValid;
    private bool isMaximumValid;

    private AffixValueSingle originalMinimum;
    private AffixValueSingle originalMaximum;

	// Use this for initialization
	void Start () {
        MinimumInput.onValueChanged.AddListener(UpdateIsMinimumChanged);
        MaximumInput.onValueChanged.AddListener(UpdateIsMaximumChanged);
	}

    private void UpdateIsChanged()
    {
        IsChanged = IsMinimumChanged || IsMaximumChanged;
    }

    private void UpdateIsMinimumChanged(string current)
    {
        if (current == "")
        {
            isMinimumValid = false;
            return;
        }
        isMaximumValid = true;

        IsMinimumChanged = float.Parse(current) != originalMinimum;
    }

    private void UpdateIsMaximumChanged(string current)
    {
        if (current == "")
        {
            isMaximumValid = false;
            return;
        }
        isMaximumValid = true;

        IsMaximumChanged = float.Parse(current) != originalMaximum;
    }

    public override void SetValueInfo(AffixValueInfo info)
    {
        if (!(info.BaseValueMin is AffixValueSingle))
            throw new ArgumentException($"Can only display AffixValueSingle info!", nameof(info));

        originalMinimum = info.BaseValueMin as AffixValueSingle;
        originalMaximum = info.BaseValueMax as AffixValueSingle;

        MinimumInput.text = originalMinimum.ToString();
        MaximumInput.text = originalMaximum.ToString();
    }

    public void SetValueInfo(Range range)
    {
        originalMinimum = range.MinValue;
        originalMaximum = range.MaxValue;

        MinimumInput.text = originalMinimum.ToString();
        MaximumInput.text = originalMaximum.ToString();
    }
}
