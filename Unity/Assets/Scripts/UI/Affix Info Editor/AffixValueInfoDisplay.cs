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

    public GameObject SingleValueInputPrefab;
    public GameObject RangeInputPrefab;

    AffixValueInfo currentInfo;
    GameObject input_go;
    AffixValueInfoInput input;

    public void SetInfo(AffixValueInfo info)
    {
        if (info == currentInfo)
            return;

        if (info.BaseValueMin is AffixValueSingle && !(input is AffixValueInfoSingleInput))
        {
            Destroy(input_go);

            input_go = Instantiate(SingleValueInputPrefab, InputContainer);
            UpdateHeights();
        }
        else if (info.BaseValueMin is AffixValueRange && !(input is AffixValueInfoRangeInput))
        {
            Destroy(input_go);

            input_go = Instantiate(RangeInputPrefab, InputContainer);
            UpdateHeights();
        }

        input = input_go.GetComponent<AffixValueInfoInput>();
        input.SetValueInfo(info);
        input.OnChangedStatusUpdated += UpdateLabel;
    }

    void UpdateHeights()
    {
        float content_height = input_go.GetComponent<LayoutElement>().minHeight;
        Layout.minHeight = 20 + content_height;
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
