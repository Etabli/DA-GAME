using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AffixInfoDisplay : MonoBehaviour
{
    public Text NameDisplay;
    public AffixDescriptionDisplay DescriptionDisplay;
    public AffixValueInfoDisplay ValueInfoDisplay;

    AffixInfo currentInfo;

    AffixValue minValue;
    AffixValue maxValue;
    string progressionFunction;
    float[] progressionParameters;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetType(AffixType type)
    {
        currentInfo = AffixInfo.GetAffixInfo(type);
        NameDisplay.text = currentInfo.Name;
        DescriptionDisplay.SetText(currentInfo.Description);
        ValueInfoDisplay.SetInfo(currentInfo.ValueInfo);
    }
}
