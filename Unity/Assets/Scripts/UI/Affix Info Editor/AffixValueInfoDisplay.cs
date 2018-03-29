using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UserDialog;

public class AffixValueInfoDisplay : MonoBehaviour
{
    public event Action<bool> OnChangedStatusUpdated;

    const string LABEL_TEXT = "Values";

    public float MaxHeight;

    public RectTransform InputContainer;
    public LayoutElement Layout;
    public LayoutElement ScrollViewLayout;
    public Text Label;
    public Dropdown TypeDropdown;

    public GameObject SingleValueInputPrefab;
    public GameObject RangeInputPrefab;
    public GameObject MultipleInputPrefab;

    #region Properties
    public bool IsChanged
    {
        get
        {
            if (input != null)
                return input.IsChanged;
            return false;
        }
    }

    public bool IsValid
    {
        get
        {
            if (input != null)
                return input.IsValid;
            return true;
        }
    }

    public AffixValueInfo Info
    {
        get
        {
            if (input != null)
                return input.Info;
            return null;
        }
    }
    #endregion

    AffixValueInfo currentInfo;
    GameObject inputGO;
    AffixValueInfoInput input;
    int previousType;

    private void Start()
    {
        Type affixValueBaseType = typeof(AffixValue);
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && t != affixValueBaseType && affixValueBaseType.IsAssignableFrom(t))
            .Select(t => t.Name.Remove(0, "AffixValue".Length));

        TypeDropdown.AddOptions(types.ToList());

        TypeDropdown.onValueChanged.AddListener(
            index =>
            {
                if (TypeDropdown.value == previousType)
                    return;

                if (IsChanged)
                {
                    DialogController.ShowBlocking("You have unsaved changes, are you sure you want to change the value type?"
                    , () =>
                    {
                        ChangeType(index);
                    }
                    , () => TypeDropdown.value = previousType);
                }
                else
                {
                    ChangeType(index);
                }
            });
    }

    public void SetInfo(AffixValueInfo info)
    {
        if (ReferenceEquals(info, currentInfo))
            return;

        currentInfo = info;

        Type type = info.BaseValueMin.GetType();
        UpdateValueDisplay(type);
        input.SetValueInfo(info);
        TypeDropdown.value = TypeDropdown.options.FindIndex(option => option.text == type.Name.Remove(0, "AffixValue".Length));
        previousType = TypeDropdown.value;
    }

    void UpdateValueDisplay(Type type)
    {
        if (type == typeof(AffixValueSingle) && !(input is AffixValueInfoSingleInput))
        {
            Destroy(inputGO);

            inputGO = Instantiate(SingleValueInputPrefab, InputContainer);
            UpdateHeights();
        }
        else if (type == typeof(AffixValueRange) && !(input is AffixValueInfoRangeInput))
        {
            Destroy(inputGO);

            inputGO = Instantiate(RangeInputPrefab, InputContainer);
            UpdateHeights();
        }
        else if (type == typeof(AffixValueMultiple) && !(input is AffixValueInfoMultipleInput))
        {
            Destroy(inputGO);

            inputGO = Instantiate(MultipleInputPrefab, InputContainer);
            var input = inputGO.GetComponent<AffixValueInfoMultipleInput>();
            input.OnHeightChanged += x => UpdateHeights();
            UpdateHeights();
        }

        var newInput = inputGO.GetComponent<AffixValueInfoInput>();
        if (!ReferenceEquals(input, newInput))
        {
            input = newInput;
            input.Initialize();
            input.OnChangedStatusUpdated += UpdateLabel;
            input.OnChangedStatusUpdated += isChanged => OnChangedStatusUpdated?.Invoke(isChanged);
        }
    }

    void ChangeType(int typeIndex)
    {
        string name = TypeDropdown.options[typeIndex].text;

        Type type = Assembly.GetExecutingAssembly().GetType("AffixValue" + name);
        if (type == null)
            throw new InvalidOperationException("AffixValue type dropdown contained non-existant type!");

        previousType = TypeDropdown.value;
        UpdateValueDisplay(type);

        if (type != currentInfo.BaseValueMin.GetType())
        {
            OnChangedStatusUpdated?.Invoke(true);
            input.OnChangedStatusUpdated -= UpdateLabel;
            UpdateLabel(true);
        }
        else
        {
            OnChangedStatusUpdated?.Invoke(false);
            input.SetValueInfo(currentInfo);
            UpdateLabel(false);
        }
    }

    void UpdateHeights()
    {
        float contentHeight = inputGO.GetComponent<LayoutElement>().minHeight;
        InputContainer.sizeDelta = new Vector2(InputContainer.sizeDelta.x, contentHeight);

        Layout.minHeight = Label.GetComponent<LayoutElement>().minHeight
            + TypeDropdown.GetComponent<LayoutElement>().minHeight
            + contentHeight
            + 10; // Spacing between elements

        Layout.minHeight = Mathf.Min(Layout.minHeight, MaxHeight); 
    }

    void UpdateLabel(bool isChanged)
    {
        if (isChanged)
            Label.text = LABEL_TEXT + "*";
        else
            Label.text = LABEL_TEXT;

    }
}
