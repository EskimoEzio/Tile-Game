using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UpgradeManager : MonoBehaviour
{

    private BlockController blockController;


    private List<IBlockUpgrade> blockUpgrades = new();
    
    private List<ITargetUpgrade> targetUpgrades = new(); // I may end up changing this as I don't know if i want targeting to factor in extra details. Ignore for now as this is mostly focusing on targeting allies
    private ITargetReplacementUpgrade targetReplacementUpgrade; //this is also stored seperatley as you can only have 1 and I want to make sure i keep track of that



    private void Awake()
    {
        blockController = GetComponent<BlockController>();
    }


    private void AddUpgrade(IBlockUpgrade upgrade)
    {
        blockUpgrades.Add(upgrade);

        if(upgrade is ITargetUpgrade targetUpgrade)
        {
            if(targetUpgrade is ITargetReplacementUpgrade replacementUpgrade)
            {
                targetUpgrades.Remove(targetReplacementUpgrade); // remove the old replacement upgrade from the list
                targetUpgrades.Add(replacementUpgrade); 
                targetReplacementUpgrade = replacementUpgrade; // set this as the new targetReplacementUpgrade
            }
            
            targetUpgrades.Add(targetUpgrade);
        }

    }


    void CheckTargetUpgrades() // Atm this is only checks target replacement
    {
        if(targetUpgrades.Count == 0) // if there are no target upgrades
        {
            blockController.BaseTarget();
            return;
        }

        if(targetReplacementUpgrade != null)
        {
            // do the new target technique
            targetReplacementUpgrade.Target(blockController);
        }
        else
        {
            blockController.BaseTarget();
        }



    }


}
