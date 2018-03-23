using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffixListDisplay : MonoBehaviour
{
    const int HEIGHT_PER_AFFIX = 20;

    public GameObject AffixTypeDisplay;
    public RectTransform RectTransform;
    public AffixInfoDisplay AffixInfoDisplay;

    AffixType selectedType = AffixType.None;

    private void Start()
    {
        foreach (var type in AffixType.Types)
        {
            var display_go = Instantiate(AffixTypeDisplay, transform);
            AffixTypeDisplay display = display_go.GetComponent<AffixTypeDisplay>();
            display.SetAffixType(type);
            display.OnClick += SelectType;
        }

        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, AffixType.Types.Count * HEIGHT_PER_AFFIX);
    }

    void SelectType(AffixType type)
    {
        if (type == selectedType)
            return;

        selectedType = type;
        AffixInfoDisplay.SetType(selectedType);
    }

    public void TestDialog()
    {
        UserDialogController.Show("hi",
            () => UserDialogController.Show("What's your name?",
                name => UserDialogController.Show($"Nice to meet you, {name}"),
                () => UserDialogController.Show("I said, what's your name?",
                    name => UserDialogController.Show($"See, was that so hard, {name}?"))),
            () => UserDialogController.ShowBlocking("I didn't wanna talk to you anyway"));
    }
}
