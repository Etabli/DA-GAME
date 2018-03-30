using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AffixDescriptionDisplay : MonoBehaviour
{
    public event Action<bool> OnChangedStatusUpdated;

    const string LABEL_TEXT = "Description";

    public Color DefaultColor;
    public Color InvalidColor;
    public Image InputBackground;
    public InputField InputField;
    public Text Label;

    private bool _isChanged;
    /// <summary>
    /// Whether or not the description has been changed from the original
    /// </summary>
    public bool IsChanged
    {
        get { return _isChanged; }
        private set
        {
            if (_isChanged != value)
            {
                _isChanged = value;

                if (_isChanged)
                    Label.text = LABEL_TEXT + "*";
                else
                    Label.text = LABEL_TEXT;

                OnChangedStatusUpdated?.Invoke(_isChanged);
            }
        }
    }

    public string Text
    {
        get
        {
            if (IsValid)
                return InputField.text;
            return null;
        }
    }

    public bool IsValid
    {
        get
        {
            return InputField.text.Contains("{0}");
        }
    }

    string originalText;

	// Use this for initialization
	void Start () {
        InputField.onValueChanged.AddListener(CheckChanged);
	}

    void CheckChanged(string currentText)
    {
        if (IsValid)
            InputBackground.color = DefaultColor;
        else
            InputBackground.color = InvalidColor;

        IsChanged = originalText != currentText;
    }

    public void SetText(string text)
    {
        originalText = text;
        InputField.text = text;
        IsChanged = false;
    }
}
