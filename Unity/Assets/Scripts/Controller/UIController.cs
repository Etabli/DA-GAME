using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Placeholder for actual controller
public class UIController : MonoBehaviour
{
    public GameObject AffixEditor;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F8))
            AffixEditor.SetActive(!AffixEditor.activeSelf);
	}
}
