using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AffixValueInfoDisplay : MonoBehaviour
{
    const string LABEL_TEXT = "Values";

    public RectTransform InputContainer;
    public LayoutElement Layout;
    public LayoutElement ScrollViewLayout;
    public Text Label;
    public Dropdown TypeDropdown;

    public GameObject SingleValueInputPrefab;
    public GameObject RangeInputPrefab;

    AffixValueInfo currentInfo;
    GameObject input_go;
    AffixValueInfoInput input;

    private void Start()
    {
        Type affixValueBaseType = typeof(AffixValue);
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && t != affixValueBaseType && affixValueBaseType.IsAssignableFrom(t))
            .Select(t => t.Name.Remove(0, "AffixValue".Length));

        TypeDropdown.AddOptions(types.ToList());

        // TODO: Add confirmation dialog here
        TypeDropdown.onValueChanged.AddListener(ChangeType);
    }

    public void SetInfo(AffixValueInfo info)
    {
        if (info == currentInfo)
            return;

        currentInfo = info;

        Type type = info.BaseValueMin.GetType();
        UpdateValueDisplay(type);
        input.SetValueInfo(info);
        TypeDropdown.value = TypeDropdown.options.FindIndex(option => option.text == type.Name.Remove(0, "AffixValue".Length));
    }

    void UpdateValueDisplay(Type type)
    {
        if (type == typeof(AffixValueSingle) && !(input is AffixValueInfoSingleInput))
        {
            Destroy(input_go);

            input_go = Instantiate(SingleValueInputPrefab, InputContainer);
            UpdateHeights();
        }
        else if (type == typeof(AffixValueRange) && !(input is AffixValueInfoRangeInput))
        {
            Destroy(input_go);

            input_go = Instantiate(RangeInputPrefab, InputContainer);
            UpdateHeights();
        }

        input = input_go.GetComponent<AffixValueInfoInput>();
        input.OnChangedStatusUpdated += UpdateLabel;
    }

    void ChangeType(int typeIndex)
    {
        string name = TypeDropdown.options[typeIndex].text;

        Type type = Assembly.GetExecutingAssembly().GetType("AffixValue" + name);
        if (type == null)
            throw new InvalidOperationException("AffixValue type dropdown contained non-existant type!");

        UpdateValueDisplay(type);

        if (type != currentInfo.BaseValueMin.GetType())
        {
            input.OnChangedStatusUpdated -= UpdateLabel;
            UpdateLabel(true);
        }
        else
        {
            input.SetValueInfo(currentInfo);
            UpdateLabel(false);
        }
    }

    void UpdateHeights()
    {
        float content_height = input_go.GetComponent<LayoutElement>().minHeight;
        Layout.minHeight = Label.GetComponent<LayoutElement>().minHeight
            + TypeDropdown.GetComponent<LayoutElement>().minHeight
            + content_height
            + 10; // Spacing between elements
        InputContainer.sizeDelta = new Vector2(InputContainer.sizeDelta.x, content_height);
        ScrollViewLayout.minHeight = content_height;
    }

    void UpdateLabel(bool isChanged)
    {
        if (isChanged)
            Label.text = LABEL_TEXT + "*";
        else
            Label.text = LABEL_TEXT;

    }
}
