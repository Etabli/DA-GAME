using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    
	// Use this for initialization
	void Start ()
    {
        AttributeInfoSerializer.LoadFromDisk(AttributeType.Health);
        AttributeInfoSerializer.LoadFromDisk(AttributeType.PhysDmgFlat);
        AttributeInfoSerializer.LoadFromDisk(AttributeType.FireRate);

        AttributePool.LoadPoolsFromDisk();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            WeaponBase wBase = new WeaponBase(ItemBaseType.Pistol ,new AttributeType[] { AttributeType.PhysDmgFlat }, new AttributeType[] { AttributeType.FireRate }, new AmmoClass[] { AmmoClass.Bullet, AmmoClass.Battery });
            Weapon w = wBase.GenerateItem(1, 1) as Weapon;
            print("Generated weapon!");
            print(w);
        }
	}
}
