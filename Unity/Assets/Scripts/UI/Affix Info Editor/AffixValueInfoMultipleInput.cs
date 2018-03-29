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

    public override AffixValueInfo Info
    {
        get
        {
            if (!IsValid)
                return null;

            List<AffixValueInfo> values = new List<AffixValueInfo>();
            foreach (var input in inputs)
                values.Add(input.Info);

            return new AffixValueInfoMultiple(values);
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
        throw new NotImplementedException();
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

        UpdateElementModifierDropdown(index);

        RemoveElementModifier();
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
        Debug.Log("hi");
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

        UpdateHeight();
        UpdateSiblingIndices();
    }
}
