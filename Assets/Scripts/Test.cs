﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    AttributeValue ProgressFloat(int tier, AttributeValue num)
    {
        return (AttributeValueSingle)num * tier;
    }



	// Use this for initialization
	void Start ()
    {
        AttributeValueInfo attrVal = new AttributeValueInfo(new AttributeValueSingle(5.0f), new AttributeValueSingle(8.0f), ProgressFloat);
        AttributeInfo attrInfo = new AttributeInfo(AttributeType.Health, AttributeValueType.SingleValue, "Base Health", attrVal);

        AttributeValueInfo physDmgFlatValInfo = new AttributeValueInfo(new AttributeValueRange(3, 4), new AttributeValueRange(5, 7), (tier, range) => range * tier);
        AttributeInfo physDmgFlatInfo = new AttributeInfo(AttributeType.PhysDmgFlat, AttributeValueType.Range, "Flat Physical Damage", physDmgFlatValInfo);
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            AttributeValueRange r = new AttributeValueRange(new Range(5, 7));
            print(-r);

            Attribute attr = AttributeInfo.GetAttributeInfo(AttributeType.PhysDmgFlat).GenerateAttribute(2) as Attribute;
            print(attr);
        }
	}
}
