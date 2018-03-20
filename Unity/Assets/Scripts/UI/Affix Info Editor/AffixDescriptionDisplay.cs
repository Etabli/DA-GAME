using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AffixDescriptionDisplay : MonoBehaviour
{
    const string LABEL_TEXT = "Description";

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
            }
        }
    }

    public string Text
    {
        get { return InputField.text; }
    }

    string originalText;

	// Use this for initialization
	void Start () {
        InputField.onValueChanged.AddListener(CheckChanged);
	}

    void CheckChanged(string currentText)
    {
        IsChanged = originalText != currentText;
    }

    public void SetText(string text)
    {
        originalText = text;
        InputField.text = text;
    }
}
