using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AffixValueInfoMultipleInput : AffixValueInfoInput
{
    public event Action<float> OnHeightChanged;

    public LayoutElement layout;
    public GameObject singleInputPrefab;
    public GameObject rangeInputPrefab;
    public GameObject listElementModifierPrefab;

    List<AffixValueInfoInput> inputs;
    List<ListElementModifier> elementModifiers;
    AffixValueInfo originalInfo;

    public override AffixValueInfo Info
    {
        get
        {
            if (!IsValid)
                return null;

            List<AffixValue> minValues = new List<AffixValue>();
            List<AffixValue> maxValues = new List<AffixValue>();
            List<AffixProgression> progressions = new List<AffixProgression>();
            foreach (var input in inputs)
            {
                var info = input.Info;
                minValues.Add(info.BaseValueMin);
                maxValues.Add(info.BaseValueMax);
                progressions.Add(info.Progression);
            }

            return new AffixValueInfo(new AffixValueMultiple(minValues.ToArray()), new AffixValueMultiple(maxValues.ToArray()), progressions.ToArray());
        }
    }

    public override bool IsValid
    {
        get
        {
            return inputs.All(i => i.IsValid);
        }
    }

    public override void Initialize()
    {
        inputs = new List<AffixValueInfoInput>();
        elementModifiers = new List<ListElementModifier>();

        CreateElementModifier();
        AddInput(0, singleInputPrefab);
    }

    public override void SetValueInfo(AffixValueInfo info)
    {
        if (!(info.BaseValueMin is AffixValueMultiple))
            throw new ArgumentException($"Info needs to be of type AffixValueMultiple!");

        originalInfo = info;

        // First clear all exisiting inputs
        while (inputs.Count > 0)
            RemoveInput(inputs.Count - 1);

        var min = info.BaseValueMin as AffixValueMultiple;
        var max = info.BaseValueMax as AffixValueMultiple;

        for (int i = 0; i < min.Count; i++)
        {
            if (!(min[i] is AffixValueSingle) && !(min[i] is AffixValueRange))
                throw new ArgumentException($"Info contains affix values other than single or range!");

            GameObject prefab = min[i] is AffixValueSingle ? singleInputPrefab : rangeInputPrefab;
            AddInput(i, prefab);
            var input = inputs.Last();
            input.SetValueInfo(new AffixValueInfo(min[i], max[i], info.Progressions[i]));
        }

        UpdateIsChanged();
    }

    /// <summary>
    /// Creates a new list element modifier and adds it as the last object
    /// </summary>
    /// <param name="index"></param>
    ListElementModifier CreateElementModifier()
    {
        var elementModifierGO = Instantiate(listElementModifierPrefab, transform);
        var elementModifier = elementModifierGO.GetComponent<ListElementModifier>();
        elementModifier.SetDropdownOptions(new List<string>() {"Single", "Range"});

        elementModifier.Index = elementModifiers.Count;
        elementModifier.OnAdd += AddInput;
        elementModifier.OnRemove += RemoveInput;
        elementModifier.OnMoveUp += MoveInputUp;
        elementModifier.OnMoveDown += MoveInputDown;
        elementModifier.OnTypeChanged += ChangeInputType;

        // Enable buttons of the new second-to-last element modifier and disable our own
        if (elementModifiers.Count > 0)
            elementModifiers.Last().SetAddButtonOnly(false);
        elementModifier.SetAddButtonOnly(true);

        elementModifiers.Add(elementModifier);
        return elementModifier;
    }

    /// <summary>
    /// Removes the last element modifier
    /// </summary>
    void RemoveElementModifier()
    {
        int index = elementModifiers.Count - 1;
        var go = elementModifiers[index].gameObject;
        elementModifiers.RemoveAt(index);
        Destroy(go);

        elementModifiers.Last().SetAddButtonOnly(true);
    }

    /// <summary>
    /// Updates the sibling indices of all inputs and element modifiers starting from a given index
    /// </summary>
    void UpdateSiblingIndices(int index)
    {
        // First is always an element modifier, then alternate
        for (int i = 0; i < inputs.Count; i++)
        {
            inputs[i].transform.SetSiblingIndex(2 * i + 1);
            elementModifiers[i + 1].transform.SetSiblingIndex(2 * i + 2);
        }
    }

    /// <summary>
    /// Updates the sibling indices of all inputs and element modifiers
    /// </summary>
    void UpdateSiblingIndices() => UpdateSiblingIndices(0);

    void UpdateHeight()
    {
        if (elementModifiers == null)
            throw new InvalidOperationException("Need to initialize multiple value input!");

        float height = 0;
        foreach (var input in inputs)
        {
            height += input.GetComponent<LayoutElement>().minHeight;
        }
        height += elementModifiers[0].GetComponent<LayoutElement>().minHeight * elementModifiers.Count;
        height += inputs.Count * GetComponent<VerticalLayoutGroup>().spacing * 2;

        layout.minHeight = height;
        OnHeightChanged?.Invoke(height);
    }

    void AddInput(int index, GameObject prefab)
    {
        var inputGO = Instantiate(prefab, transform);
        var input = inputGO.GetComponent<AffixValueInfoInput>();
        input.Initialize();
        input.OnChangedStatusUpdated += _ => UpdateIsChanged();

        inputs.Insert(index, input);
        CreateElementModifier();

        for (int i = index; i < inputs.Count; i++)
            UpdateElementModifierDropdown(i);

        UpdateSiblingIndices(index);
        UpdateHeight();
    }

    void AddInput(int index) => AddInput(index, singleInputPrefab);

    /// <summary>
    /// Removes the input at the given index and its associated element modifier
    /// </summary>
    /// <param name="index"></param>
    void RemoveInput(int index)
    {
        var input = inputs[index];
        inputs.RemoveAt(index);
        Destroy(input.gameObject);
        RemoveElementModifier();

        UpdateElementModifierDropdown(index);

        UpdateHeight();
        UpdateSiblingIndices();
    }

    void MoveInputUp(int index) => SwitchInputs(index, index - 1);

    void MoveInputDown(int index) => SwitchInputs(index, index + 1);

    /// <summary>
    /// Switches the inputs with the two given indices
    /// </summary>
    void SwitchInputs(int index1, int index2)
    {
        if (index1 == index2 || index1 < 0 || index2 < 0 || index1 >= inputs.Count || index2 >= inputs.Count)
            return;

        var input1 = inputs[index1];
        var input2 = inputs[index2];
        inputs[index1] = input2;
        inputs[index2] = input1;
        UpdateElementModifierDropdown(index1);
        UpdateElementModifierDropdown(index2);

        UpdateSiblingIndices();
    }

    /// <summary>
    /// Updates the value of the element modifier dropdown at the specified index to display
    /// the correct type
    /// </summary>
    /// <param name="index"></param>
    void UpdateElementModifierDropdown(int index)
    {
        if (index >= inputs.Count)
            return;

        var input = inputs[index];
        var elementModifier = elementModifiers[index];

        int newValue = input is AffixValueInfoSingleInput ? 0 : 1;
        if (elementModifier.TypeDropdown.value != newValue)
            elementModifier.SetDropdownValue(newValue);
    }

    /// <summary>
    /// Changes the type of input at the specified index
    /// </summary>
    void ChangeInputType(int index, string type)
    {
        var input = inputs[index];

        // First check if type changed at all
        if ((type == "Single" && input is AffixValueInfoRangeInput)
            || (type == "Range" && input is AffixValueInfoSingleInput))
        {
            // Show confirmation dialog if there are changes
            if (input.IsChanged)
                UserDialog.DialogController.ShowBlocking("There are unsaved changes. Are you sure you want to change the type?"
                    , () => ToggleInputType(index)
                    , () => UpdateElementModifierDropdown(index));
            else
                ToggleInputType(index);
        }
    }

    /// <summary>
    /// Toggles the type of the input at the specified index
    /// </summary>
    /// <param name="index"></param>
    void ToggleInputType(int index)
    {
        var input = inputs[index];

        GameObject prefab = input is AffixValueInfoSingleInput ? rangeInputPrefab : singleInputPrefab;
        var newInput = Instantiate(prefab, transform).GetComponent<AffixValueInfoInput>();
        newInput.Initialize();
        inputs[index] = newInput;
        Destroy(input.gameObject);

        var originalMin = (originalInfo.BaseValueMin as AffixValueMultiple)[index];
        if ((originalMin is AffixValueSingle && newInput is AffixValueInfoSingleInput)
            || (originalMin is AffixValueRange && newInput is AffixValueInfoRangeInput))
        {
            var originalMax = (originalInfo.BaseValueMax as AffixValueMultiple)[index];
            newInput.SetValueInfo(new AffixValueInfo(originalMin, originalMax, originalInfo.Progressions[index]));
        }

        UpdateHeight();
        UpdateSiblingIndices();
        UpdateIsChanged();
    }

    void UpdateIsChanged()
    {
        if (originalInfo == null && !IsValid)
        {
            IsChanged = true;
            return;
        }

        var min = originalInfo.BaseValueMin as AffixValueMultiple;
        var max = originalInfo.BaseValueMax as AffixValueMultiple;
        for (int i = 0; i < inputs.Count; i++)
        {
            var input = inputs[i];
            if ((input is AffixValueInfoSingleInput && min[i] is AffixValueRange)
                || (input is AffixValueInfoRangeInput && min[i] is AffixValueSingle))
            {
                IsChanged = true;
                return;
            }
            else
            {
                if (input.IsChanged)
                {
                    IsChanged = true;
                    return;
                }
            }
        }

        IsChanged = false;
    }
}
