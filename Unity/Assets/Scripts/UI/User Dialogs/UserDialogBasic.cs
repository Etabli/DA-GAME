using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDialogBasic : UserDialog
{
    [SerializeField]
    protected Button OKButton;

    protected override void Awake()
    {
        base.Awake();

        if (OKButton == null)
            Debug.LogError("User dialog's reference to OK button is not set!");
        else
        {
            OKButton.onClick.AddListener(Close);
        }
    }
}
