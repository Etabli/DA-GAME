using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    
	// Use this for initialization
	void Start ()
    {
        //AttributeValueInfo attrVal = new AttributeValueInfo(new AttributeValueSingle(5.0f), new AttributeValueSingle(8.0f), new AttributeProgression("Linear", new float[] { 0, 1 }));
        //AttributeInfo attrInfo = new AttributeInfo(AttributeType.Health, AttributeValueType.SingleValue, "Base Health", attrVal, "Adds {0} Health");

        //AttributeInfoSerializer.SaveToDisk(attrInfo);

        AttributeInfoSerializer.LoadFromDisk(AttributeType.Health);

        //AttributeValueInfo physDmgFlatValInfo = new AttributeValueInfo(new AttributeValueRange(3, 4), new AttributeValueRange(5, 7), new AttributeProgression("Linear", new float[] { 0, 1 }));
        //AttributeInfo physDmgFlatInfo = new AttributeInfo(AttributeType.PhysDmgFlat, AttributeValueType.Range, "Flat Physical Damage", physDmgFlatValInfo);

        //AttributeInfoSerializer.SaveToDisk(physDmgFlatInfo);

        AttributeInfoSerializer.LoadFromDisk(AttributeType.PhysDmgFlat);
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            AttributeInfo info = AttributeInfo.GetAttributeInfo(AttributeType.PhysDmgFlat);
            print(info);

            Attribute attr = info.GenerateAttribute(7);
            print(attr);


        }
	}
}
