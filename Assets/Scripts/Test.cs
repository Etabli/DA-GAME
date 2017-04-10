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

        //CreateTestPools();
        AttributePool.LoadPoolsFromDisk();
    }

    void CreateTestPools()
    {
        AttributePool weaponPool = new AttributePool(AttributeType.FireRate, AttributeType.PhysDmgFlat, AttributeType.Health);
        AttributePool armorPool = new AttributePool(AttributeType.Health, AttributeType.PhysDmgFlat);
        AttributePool ammoPool = new AttributePool(AttributeType.PhysDmgFlat);

        AttributePool.RegisterPresetPool(AttributePoolPreset.Ammo, ammoPool);
        AttributePool.RegisterPresetPool(AttributePoolPreset.Armor, armorPool);
        AttributePool.RegisterPresetPool(AttributePoolPreset.Weapon, weaponPool);

        AttributePool.SavePoolsToDisk();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            //WeaponBase wBase = new WeaponBase(ItemBaseType.Pistol ,new AttributeType[] { AttributeType.PhysDmgFlat }, new AttributeType[] { AttributeType.FireRate }, new AmmoClass[] { AmmoClass.Bullet, AmmoClass.Battery });
            //Weapon w = wBase.GenerateItem(1, 1) as Weapon;
            //print("Generated weapon!");
            //print(w);

            Lottery<string> l = new Lottery<string>();
            l.Enter("first", 1);
            l.Enter("second", 1);
            l.Enter("third", 1);
            l.Enter("fourth", 1);

            print(l.GetWinner(new HashSet<string>() { "second", "fourth" }));

            
        }
	}
}
