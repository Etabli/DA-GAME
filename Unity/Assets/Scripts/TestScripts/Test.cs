using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Serializer.LoadAttributeInfoFromDisk(AttributeType.Health);
        Serializer.LoadAttributeInfoFromDisk(AttributeType.PhysDmgFlat);
        Serializer.LoadAttributeInfoFromDisk(AttributeType.FireRate);

        //CreateTestPools();
        AttributePool.LoadPresets();
    }

    void CreateTestPools()
    {
        AttributePool weaponPool = new AttributePool(AttributeType.FireRate, AttributeType.PhysDmgFlat, AttributeType.Health);
        AttributePool armorPool = new AttributePool(AttributeType.Health, AttributeType.PhysDmgFlat);
        AttributePool ammoPool = new AttributePool(AttributeType.PhysDmgFlat);

        AttributePool.RegisterPresetPool(AttributePoolPreset.Ammo, ammoPool);
        AttributePool.RegisterPresetPool(AttributePoolPreset.Armor, armorPool);
        AttributePool.RegisterPresetPool(AttributePoolPreset.Weapon, weaponPool);

        AttributePool.SavePresets();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WeaponBase wBase = new WeaponBase(ItemBaseType.Pistol, new AttributeType[] { AttributeType.PhysDmgFlat }, new AttributeType[] { AttributeType.FireRate }, new AmmoClass[] { AmmoClass.Bullet, AmmoClass.Battery }, AttributePoolPreset.Weapon);
            Weapon w = wBase.GenerateItem(7, 15) as Weapon;
            print(w);
        }
    }
}
