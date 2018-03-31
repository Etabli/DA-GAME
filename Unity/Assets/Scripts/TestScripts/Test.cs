using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Debug = UnityEngine.Debug;

public class Test : MonoBehaviour
{
    void Awake()
    {
        //AffixInfo healthInfo = new AffixInfo("Health", new AffixValueInfo(3.0f, 5.0f, new AffixProgression("Linear", 0, 1)), "Increases your health by {0}");
        //AffixInfo physDmgFlatInfo = new AffixInfo("Flat Physical Damage", new AffixValueInfo(new AffixValueRange(1, 1.3f), new AffixValueRange(1.5f, 2f)), "Increases your physical damage by {0}");
        //AffixInfo fireRateInfo = new AffixInfo("Fire Rate", new AffixValueInfo(0.5f, 0.8f), "Increases your fire rate by {0}");

        //Serializer.SaveAffixInfoToDisk(healthInfo);
        //Serializer.SaveAffixInfoToDisk(physDmgFlatInfo);
        //Serializer.SaveAffixInfoToDisk(fireRateInfo);

        Serializer.LoadAllAffixInfosFromDisk();

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

    void GenerateTestWeapon()
    {
        WeaponBase wBase = new WeaponBase(ItemBaseType.Pistol, new AffixType[] { AffixType.PhysDmgFlat }, new AffixType[] { AffixType.FireRate }, new AmmoClass[] { AmmoClass.Bullet, AmmoClass.Battery }, AffixPoolPreset.Weapon);
        Weapon w = wBase.GenerateItem(7, 15) as Weapon;
        print(w);
    }

    void GenerateTestAffixContainer()
    {
        AffixContainer container = new AffixContainer();
        Affix health1 = AffixInfo.GetAffixInfo(AffixType.Health).GenerateAffix(2);
        container.Add(health1);
        container.Add(AffixInfo.GetAffixInfo(AffixType.Health).GenerateAffix(2));
        container.Add(health1);
        container.Add(AffixInfo.GetAffixInfo(AffixType.PhysDmgFlat).GenerateAffix(3));

        print(container);

        container.Remove(health1);

        print(container);

        Affix fireRate = AffixInfo.GetAffixInfo(AffixType.FireRate).GenerateAffix(5);
        container.Remove(fireRate);

        Affix health2 = AffixInfo.GetAffixInfo(AffixType.Health).GenerateAffix(3);
        container.Remove(health2);
    }

    void TestAffixContainerGraph()
    {
        AffixContainer c1 = new AffixContainer(new Affix[] { AffixInfo.GenerateAffix(AffixType.Health, 4) });
        AffixContainer c2 = new AffixContainer(new Affix[] { AffixInfo.GenerateAffix(AffixType.FireRate, 4) });
        AffixContainer c3 = new AffixContainer(new Affix[] { AffixInfo.GenerateAffix(AffixType.Health, 20) });
        AffixContainer c4 = new AffixContainer(new Affix[] { AffixInfo.GenerateAffix(AffixType.PhysDmgFlat, 4) });

        print(c1);
        print(c2);
        print(c3);
        print(c4);

        //c1.AppendChild(c1);

        c1.AppendChild(c2);
        print("Appended c2 to c1");
        print(c1);
        //c2.AppendChild(c1);

        c2.AppendChild(c3);
        print("Appended c3 to c2");
        print(c1);
        print(c2);
        //c3.AppendChild(c1);

        //c3.AppendChild(c2);

        c1.AppendChild(c4);
        print("Appendecd c4 to c1");
        print(c1);

        c2.DisconnectFromParent();
        print("Disconnected c2 from c1");
        print(c1);
    }

    void TestAffixGeneration()
    {
        print(AffixInfo.GenerateAffix(3, 5).ToString());
    }

    void TestLotteryPerformance()
    {
        Stopwatch timer = new Stopwatch();

        // setup
        Lottery<int> lottery = new Lottery<int>(1000000);
        HashSet<int> blacklist = new HashSet<int>();
        for (int i = 0; i < 10000; i++)
        {
            lottery.Enter(i % 100, 100);
            if (i != 69)
                blacklist.Add(i);
        }
        lottery.StartBatchDraw();

        timer.Start();

        // actual test
        lottery.Draw(100);

        timer.Stop();
        Debug.Log($"Test took {timer.Elapsed.ToString("fffffff").Insert(3, ".").TrimStart('0')} ms");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateTestWeapon();
        }
    }
}
