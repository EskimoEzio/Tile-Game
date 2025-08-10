using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UpgradeManager : MonoBehaviour
{

    private List<IBlockUpgrade> blockUpgrades = new();
    private List<ITargetUpgrade> targetUpgrades = new();



    private void AddUpgrade(IBlockUpgrade upgrade)
    {
        blockUpgrades.Add(upgrade);
    }




}
