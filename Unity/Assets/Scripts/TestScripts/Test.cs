using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //AffixInfo healthInfo = new AffixInfo(AffixType.Health, AffixValueType.SingleValue, "Health", new AffixValueInfo(3.0f, 3.0f, new AffixProgression("Linear", 0, 1)), "Increases your health by {0}");
        //AffixInfo physDmgFlatInfo = new AffixInfo(AffixType.PhysDmgFlat, AffixValueType.Range, "Flat Physical Damage", new AffixValueInfo(new AffixValueRange(1, 1.3f), new AffixValueRange(1.5f, 2f)), "Increases your physical damage by {0}");
        //AffixInfo fireRateInfo = new AffixInfo(AffixType.FireRate, AffixValueType.SingleValue, "Fire Rate", new AffixValueInfo(0.5f, 0.8f), "Increases your fire rate by {0}");

        //Serializer.SaveAffixInfoToDisk(healthInfo);
        //Serializer.SaveAffixInfoToDisk(physDmgFlatInfo);
        //Serializer.SaveAffixInfoToDisk(fireRateInfo);

        Serializer.LoadAffixInfoFromDisk(AffixType.Health);
        Serializer.LoadAffixInfoFromDisk(AffixType.PhysDmgFlat);
        Serializer.LoadAffixInfoFromDisk(AffixType.FireRate);

        //CreateTestPools();
        AffixPool.LoadPresets();
    }

    void CreateTestPools()
    {
        AffixPool weaponPool = new AffixPool(AffixType.FireRate, AffixType.PhysDmgFlat, AffixType.Health);
        AffixPool armorPool = new AffixPool(AffixType.Health, AffixType.PhysDmgFlat);
        AffixPool ammoPool = new AffixPool(AffixType.PhysDmgFlat);

        AffixPool.RegisterPresetPool(AffixPoolPreset.Ammo, ammoPool);
        AffixPool.RegisterPresetPool(AffixPoolPreset.Armor, armorPool);
        AffixPool.RegisterPresetPool(AffixPoolPreset.Weapon, weaponPool);

        AffixPool.SavePresets();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WeaponBase wBase = new WeaponBase(ItemBaseType.Pistol, new AffixType[] { AffixType.PhysDmgFlat }, new AffixType[] { AffixType.FireRate }, new AmmoClass[] { AmmoClass.Bullet, AmmoClass.Battery }, AffixPoolPreset.Weapon);
            Weapon w = wBase.GenerateItem(7, 15) as Weapon;
            print(w);
        }
    }
}
