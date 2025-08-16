using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UpgradeManager : MonoBehaviour
{

    private BlockController blockController;


    private List<BlockUpgrade> blockUpgrades = new();
    
    //Target
    private List<ITargetUpgrade> targetUpgrades = new(); // I may end up changing this as I don't know if i want targeting to factor in extra details. Ignore for now as this is mostly focusing on targeting allies
    private ITargetReplacementUpgrade targetReplacementUpgrade; //this is also stored seperatley as you can only have 1 and I want to make sure i keep track of that


    //Attack
    private List<IAttackUpgrade> attackUpgrades = new();


    //Attack Condition
    private List<IAttackConditionUpgrade> attackConditionUpgrades = new();
    public bool OverwriteBaseAllyCheck { get; private set; } = false;
    public bool OverwriteBaseNilPowerCheck { get; private set; } = false;



    private void Awake()
    {
        blockController = GetComponent<BlockController>();
    }

    private void Start()
    {

        AddUpgrade(new OnlyHitAllyUpgrade());

    }


    private void AddUpgrade(BlockUpgrade upgrade)
    {
        blockUpgrades.Add(upgrade);

        // I am not using a switch statement becuase I am planning for upgrades to be able to be multiple types, so every one needs to be checked per upgrade
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

        if (upgrade is IAttackUpgrade attackUpgrade)
        {
            attackUpgrades.Add(attackUpgrade);
        }

        if( upgrade is IAttackConditionUpgrade attackConditionUpgrade)
        {
            attackConditionUpgrades.Add(attackConditionUpgrade);
        }
    }


    public void CheckTargetUpgrades() // Atm this is only checks target replacement
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

    public void CheckAttackUpgrades() // Atm this does nothing, as there are no attack upgrades
    {
        if (attackUpgrades.Count == 0) // if there are no upgrades
        {
            blockController.BaseTarget();
            return;
        }
        else
        {
            print("There is an attack upgrade");
        }

    }

    /// <summary>
    /// This returns true if all attack conditons within the upgrades have been met
    /// </summary>
    /// <param name="targetBlockController"></param>
    /// <returns></returns>
    public bool CheckAttackConditionUpgrades(BlockController targetBlockController) // I may choose to separate atack conditions so that they arte not a subset of attackUpgrades as i suspect they will be handled completly differently
    {
        foreach (IAttackConditionUpgrade conditionUpgrade in attackConditionUpgrades)
        {

            print(conditionUpgrade.OverwriteAllyCheck);
            if (conditionUpgrade.OverwriteAllyCheck)
            {
                OverwriteBaseAllyCheck = true;
            }

            if (conditionUpgrade.OverwriteNilPowerCheck)
            {
                OverwriteBaseNilPowerCheck = true;
            }


            if (!conditionUpgrade.CheckCanAttack(blockController, targetBlockController)) // if any of the checks fail, the attack will fail
            {
                return false;
            }

        }

        


        return true; // if it gets to this point, that means that none of the checks in the upgrades have failed
    }

}
