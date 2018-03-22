using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AffixValueInfoRangeInput : AffixValueInfoInput
{
    const string MINIMUM_LABEL_TEXT = "Minimum";
    const string MAXIMUM_LABEL_TEXT = "Maximum";

    public override AffixValueInfo Info
    {
        get
        {
            if (!IsValid)
                return null;
            return new AffixValueInfo(
                new AffixValueRange(MinimumInput.Minimum, MinimumInput.Maximum),
                new AffixValueRange(MaximumInput.Minimum, MaximumInput.Maximum),
                MinimumInput.Progression);
        }
    }

    public override bool IsValid
    {
        get
        {
            return MinimumInput.IsValid && MaximumInput.IsValid;
        }
    }

    public AffixValueInfoSingleInput MinimumInput;
    public AffixValueInfoSingleInput MaximumInput;
    public Text MinimumLabel;
    public Text MaximumLabel;

    private void Start()
    {
        MinimumInput.OnChangedStatusUpdated += _ => UpdateIsChanged();
        MaximumInput.OnChangedStatusUpdated += _ => UpdateIsChanged();
    }

    void UpdateIsChanged()
    {
        IsChanged = MinimumInput.IsChanged || MaximumInput.IsChanged;

        if (MinimumInput.IsChanged)
            MinimumLabel.text = MINIMUM_LABEL_TEXT + "*";
        else
            MinimumLabel.text = MINIMUM_LABEL_TEXT;

        if (MaximumInput.IsChanged)
            MaximumLabel.text = MAXIMUM_LABEL_TEXT + "*";
        else
            MaximumLabel.text = MAXIMUM_LABEL_TEXT;
    }

    public override void SetValueInfo(AffixValueInfo info)
    {
        if (!(info.BaseValueMin is AffixValueRange))
            throw new ArgumentException("Can only display AffixValueRange info!", nameof(info));

        MinimumInput.SetValueInfo(info.BaseValueMin as AffixValueRange);
        MaximumInput.SetValueInfo(info.BaseValueMax as AffixValueRange);

        MinimumInput.Progression = info.Progression;
    }

    public override void Initialize()
    {
        MinimumInput.Initialize();
        MaximumInput.Initialize();
    }
}
