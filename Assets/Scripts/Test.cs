using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    
	// Use this for initialization
	void Start ()
    {
        AttributeInfoSerializer.LoadFromDisk(AttributeType.Health);
        AttributeInfoSerializer.LoadFromDisk(AttributeType.PhysDmgFlat);

        //AttributeValueInfo fireRateValInfo = new AttributeValueInfo(new AttributeValueSingle(0.05f), new AttributeValueSingle(0.1f), new AttributeProgression("Linear", 0, 1));
        //AttributeInfo fireRateInfo = new AttributeInfo(AttributeType.FireRate, AttributeValueType.SingleValue, "Fire Rate", fireRateValInfo, "Increases fire rate by {0}");
        //AttributeInfoSerializer.SaveToDisk(fireRateInfo);
        AttributeInfoSerializer.LoadFromDisk(AttributeType.FireRate);
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            //AttributePool pool1 = new AttributePool(AttributeType.FireRate, AttributeType.PhysDmgFlat);
            //AttributePool pool2 = new AttributePool(AttributeType.Health);
            //AttributePool.RegisterPoolForSlot(Slot.Weapon, pool1);
            //AttributePool.RegisterPoolForSlot(Slot.Crotch, pool2);
            //AttributePool.SavePoolsToDisk();
            AttributePool.LoadPoolsFromDisk();
            print(AttributePool.GetPool(Slot.Weapon));
        }
	}
}
