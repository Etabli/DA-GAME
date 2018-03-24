using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(Button))]
public class AffixTypeDisplay : MonoBehaviour
{
    public event Action<AffixType> OnClick;

    public Button Button;
    public Text ButtonText;

    private AffixType affixType;

    private void Start()
    {
        Button.onClick.AddListener(Click);
    }

    public void SetAffixType(AffixType type)
    {
        affixType = type;
        ButtonText.text = type.Name;
    }

    public void Select()
    {
        Button.Select();
    }

    public void SetChanged(bool value)
    {
        if (value)
            ButtonText.text = affixType.Name + "*";
        else
            ButtonText.text = affixType.Name;
    }

    private void Click()
    {
        OnClick?.Invoke(affixType);
    }
}
