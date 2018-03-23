using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDialogCancellable : UserDialogBasic
{
    public event Action OnAccept;
    public event Action OnCancel;

    [SerializeField]
    Button CancelButton;

    protected override void Awake()
    {
        base.Awake();

        if (CancelButton == null)
            throw new UserDialogException("User dialogs's reference to Cancel button is not set!");

        CancelButton.onClick.AddListener(Close);
        CancelButton.onClick.AddListener(Cancel);
        OKButton.onClick.AddListener(Accept);
    }

    void Accept() => OnAccept?.Invoke();
    void Cancel() => OnCancel?.Invoke();
}
