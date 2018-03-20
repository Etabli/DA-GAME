using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffixListDisplay : MonoBehaviour
{
    const int HEIGHT_PER_AFFIX = 30;

    public GameObject AffixTypeDisplay;
    public RectTransform RectTransform;

    private void Start()
    {
        foreach (var type in AffixType.Types)
        {
            var display_go = Instantiate(AffixTypeDisplay, transform);
            AffixTypeDisplay display = display_go.GetComponent<AffixTypeDisplay>();
            display.SetAffixType(type);
            display.OnClick += (t) => Debug.Log(t.Name);
        }

        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, AffixType.Types.Count * HEIGHT_PER_AFFIX);
    }
}
