using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Use to add support for adding support for adding/removing/changing the order of
/// elements in a list of stuff. Holds an index and fires appropriate events with
/// that index as a parameter.
/// </summary>
public class ListElementModifier : MonoBehaviour
{
    public event Action<int> OnAdd;
    public event Action<int> OnRemove;
    public event Action<int, string> OnTypeChanged;
    public event Action<int> OnMoveDown;
    public event Action<int> OnMoveUp;

    public Button AddButton;
    public Button RemoveButton;
    public Button MoveDownButton;
    public Button MoveUpButton;
    public Dropdown TypeDropdown;

    public int Index;

    private void Start()
    {
        AddButton.onClick.AddListener(() => OnAdd?.Invoke(Index));
        RemoveButton.onClick.AddListener(() => OnRemove?.Invoke(Index));
        MoveDownButton.onClick.AddListener(() => OnMoveDown?.Invoke(Index));
        MoveUpButton.onClick.AddListener(() => OnMoveUp?.Invoke(Index));
        TypeDropdown.onValueChanged.AddListener(FireTypeChangedEvent);
    }

    void FireTypeChangedEvent(int type)
    {
        OnTypeChanged?.Invoke(Index, TypeDropdown.options[type].text);
    }

    public void SetDropdownOptions(List<string> options)
    {
        TypeDropdown.options.Clear();
        TypeDropdown.AddOptions(options);
    }

    /// <summary>
    /// Sets the dropdown value without triggering the associated event
    /// </summary>
    /// <param name="value"></param>
    public void SetDropdownValue(int value)
    {
        TypeDropdown.onValueChanged.RemoveAllListeners();
        TypeDropdown.value = value;
        TypeDropdown.onValueChanged.AddListener(FireTypeChangedEvent);
    }

    public void DisableDropdown()
    {
        SetDropdownActive(false);
    }

    public void EnableDropdown()
    {
        SetDropdownActive(true);
    }

    public void SetDropdownActive(bool value)
    {
        TypeDropdown.gameObject.SetActive(value);
    }

    /// <summary>
    /// Sets whether or not this element modifier only shows the add button
    /// </summary>
    /// <param name="value"></param>
    public void SetAddButtonOnly(bool value)
    {
        RemoveButton.gameObject.SetActive(!value);
        TypeDropdown.gameObject.SetActive(!value);
        MoveUpButton.gameObject.SetActive(!value);
        MoveDownButton.gameObject.SetActive(!value);
    }
}
