using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AffixValueInfoSingleInput : AffixValueInfoInput
{
    public InputField MinimumInput;
    public InputField MaximumInput;

    public Transform ProgressionTransform;
    public Dropdown ProgressionDropdown;
    public LayoutElement ProgressionSpaceFiller;
    public GameObject ProgressionParemeterInputPrefab;

    private bool isMinimumValid;
    private bool isMaximumValid;
    private bool isProgressionValid;

    private AffixValueSingle originalMinimum;
    private AffixValueSingle originalMaximum;

    private List<InputField> progressionParameterInputs;
    private List<float> originalParameters = new List<float>();
    private string originalProgressionName;

    #region Properties
    public override AffixValueInfo Info
    {
        get
        {
            if (IsValid)
            {
                return new AffixValueInfo(float.Parse(MinimumInput.text), float.Parse(MaximumInput.text), Progression);
            }
            return null;
        }
    }

    public override bool IsValid
    {
        get
        {
            return isMinimumValid && isMaximumValid && isProgressionValid;
        }
    }

    public AffixProgression Progression
    {
        get
        {
            if (isProgressionValid)
            {
                List<float> parameters = new List<float>();
                foreach (var input in progressionParameterInputs)
                {
                    parameters.Add(float.Parse(input.text));
                }
                string name = ProgressionDropdown.options[ProgressionDropdown.value].text;

                AffixProgression progression = new AffixProgression(name, parameters.ToArray());
                return progression;
            }
            return new AffixProgression();
        }
        set
        {
            ProgressionDropdown.value = ProgressionDropdown.options.FindIndex(option => option.text == value.Name);
            UpdateProgression(value.Name);

            originalProgressionName = value.Name;
            originalParameters.Clear();
            for (int i = 0; i < progressionParameterInputs.Count; i++)
            {
                originalParameters.Add(value.Parameters[i]);
                progressionParameterInputs[i].text = value.Parameters[i].ToString();
            }
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
    #endregion

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

    private bool _isProgressionChanged;
    private bool IsProgressionChanged
    {
        get { return _isProgressionChanged; }
        set
        {
            if (_isProgressionChanged != value)
            {
                _isProgressionChanged = value;
                UpdateIsChanged();
            }
        }
    }

    private void UpdateIsChanged()
    {
        IsChanged = IsMinimumChanged || IsMaximumChanged || IsProgressionChanged;
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

    private void UpdateIsProgressionChanged()
    {
        if (progressionParameterInputs == null)
        {
            isProgressionValid = true;
            return;
        }

        // First check if valid so we don't have to worry when parsing
        for (int i = 0; i < progressionParameterInputs.Count; i++)
        {
            var input = progressionParameterInputs[i];

            if (input.text == "")
            {
                isProgressionValid = false;
                return;
            }
        }
        isProgressionValid = true;

        // First check if we are still the same progression type
        if (ProgressionDropdown.options[ProgressionDropdown.value].text != originalProgressionName)
        {
            IsProgressionChanged = true;
            return;
        }

        // Then compare each parameter
        for (int i = 0; i < progressionParameterInputs.Count; i++)
        {
            var input = progressionParameterInputs[i];
            if (float.Parse(input.text) != originalParameters[i])
            {
                IsProgressionChanged = true;
                return;
            }
        }

        IsProgressionChanged = false;
    }
    #endregion

    private void UpdateProgression(string name)
    {
        if (!AffixProgression.ParameterRequirements.ContainsKey(name))
            throw new ArgumentException("No parameter info for this progression function!");

        int nParams = AffixProgression.ParameterRequirements[name];
        if (progressionParameterInputs == null)
        {
            progressionParameterInputs = new List<InputField>(nParams);
            for (int i = 0; i < nParams; i++)
            {
                progressionParameterInputs.Add(CreateNewProgressionInput());
            }
        }

        // Either remove or add inputs until we have the desired number
        while (progressionParameterInputs.Count > nParams)
        {
            InputField input = progressionParameterInputs[progressionParameterInputs.Count - 1];
            progressionParameterInputs.RemoveAt(progressionParameterInputs.Count - 1);
            Destroy(input.gameObject);
        }
        while (progressionParameterInputs.Count < nParams)
        {
            progressionParameterInputs.Add(CreateNewProgressionInput());
        }

        // Fill back in with original parameters if we changed back to the same progression
        if (name == originalProgressionName)
        {
            for (int i = 0; i < progressionParameterInputs.Count; i++)
            {
                progressionParameterInputs[i].text = originalParameters[i].ToString();
            }
        }

        ProgressionSpaceFiller.transform.SetAsLastSibling();
    }

    private InputField CreateNewProgressionInput()
    {
        InputField new_input = Instantiate(ProgressionParemeterInputPrefab, ProgressionTransform).GetComponent<InputField>();
        new_input.onValueChanged.AddListener(_ => UpdateIsProgressionChanged());
        return new_input;
    }

    public override void Initialize()
    {
        InitializeProgressionFunctions();

        ProgressionDropdown.AddOptions(progressionFunctions);
        ProgressionDropdown.onValueChanged.AddListener(i => UpdateProgression(progressionFunctions[i]));
        ProgressionDropdown.onValueChanged.AddListener(_ => UpdateIsProgressionChanged());

        MinimumInput.onValueChanged.AddListener(UpdateIsMinimumChanged);
        MaximumInput.onValueChanged.AddListener(UpdateIsMaximumChanged);
    }

    public override void SetValueInfo(AffixValueInfo info)
    {
        if (!(info.BaseValueMin is AffixValueSingle))
            throw new ArgumentException($"Can only display AffixValueSingle info!", nameof(info));

        originalMinimum = info.BaseValueMin as AffixValueSingle;
        originalMaximum = info.BaseValueMax as AffixValueSingle;

        MinimumInput.text = originalMinimum.ToString();
        MaximumInput.text = originalMaximum.ToString();

        Progression = info.Progression;
    }

    public void SetValueInfo(Range range)
    {
        originalMinimum = range.MinValue;
        originalMaximum = range.MaxValue;

        MinimumInput.text = originalMinimum.ToString();
        MaximumInput.text = originalMaximum.ToString();
    }
}
