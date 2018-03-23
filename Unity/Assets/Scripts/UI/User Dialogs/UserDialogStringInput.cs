using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDialogStringInput : UserDialogCancellable
{
    public event Action<string> OnSubmit;

    [SerializeField]
    Text placeholderText;
    [SerializeField]
    InputField inputField;

    public string PlaceholderText
    {
        set { placeholderText.text = value; }
    }

    protected override void Awake()
    {
        base.Awake();

        if (placeholderText == null)
            throw new UserDialogException("User dialogs's reference to placeholder text is not set!");
        if (inputField == null)
            throw new UserDialogException("User dialogs's reference to input field is not set!");

        OKButton.onClick.RemoveAllListeners();
        OKButton.onClick.AddListener(Submit);

        inputField.onEndEdit.AddListener(text =>
        {
            if (Input.GetKeyDown(KeyCode.Return))
                Submit();
        });
    }

    void Submit()
    {
        if (inputField.text != "")
        {
           OnSubmit?.Invoke(inputField.text);
           Close();
        }
    }

    public void DisableCancelling()
    {
        CancelButton.gameObject.SetActive(false);
    }

    public void EnableCancelling()
    {
        CancelButton.gameObject.SetActive(true);
    }

    public void SetContentType(InputField.ContentType type)
    {
        inputField.contentType = type;
    }
}
