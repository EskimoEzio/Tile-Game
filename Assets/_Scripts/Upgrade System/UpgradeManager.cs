using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // This is necessary for the sorting of the lists by two values
public class UpgradeManager : MonoBehaviour
{
    
    private BlockController blockController;


    //keep in mind that any additions to this will need to be reflected i nthe AddUpgrad and RemoveUpgrade methods
    #region Upgrade Lists & Fields 
    private List<BlockUpgrade> blockUpgrades = new();

    //Stats
    private List<IStatUpgrade> statUpgrades = new();

    //Turn Start
    private List<ITurnStartUpgrade> turnStartUpgrades = new();

    //Turn End
    private List<ITurnEndUpgrade> turnEndUpgrades = new();


    //Place
    private List<IPlaceUpgrade> placeUpgrades = new();


    //Target
    private List<ITargetUpgrade> targetUpgrades = new(); // I may end up changing this as I don't know if i want targeting to factor in extra details. Ignore for now as this is mostly focusing on targeting allies
    private ITargetReplacementUpgrade targetReplacementUpgrade; //this is also stored seperatley as you can only have 1 and I want to make sure i keep track of that

    //Attack Condition
    private List<IAttackConditionUpgrade> attackConditionUpgrades = new();
    public bool OverwriteBaseAllyCheck { get; private set; } = false;
    public bool OverwriteBaseNilPowerCheck { get; private set; } = false;

    //Attack
    private List<IAttackUpgrade> attackUpgrades = new();

    //OnHit
    private List<IOnHitUpgrade> onHitUpgrades = new();


    //GetHit
    private List<IGetHitUpgrade> getHitUpgrades = new();

    #endregion 

    private void OnEnable()
    {
        TurnManager.Instance.OnTurnStarted += CheckTurnStartUpgrades;
        TurnManager.Instance.OnTurnEnded += CheckTurnEndUpgrades;
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnTurnStarted -= CheckTurnStartUpgrades;
        TurnManager.Instance.OnTurnEnded -= CheckTurnEndUpgrades;
    }



    private void Awake()
    {
        blockController = GetComponent<BlockController>();


        // Test for upgrades
        //AddUpgrade(new TrampleUpgrade());
        //AddUpgrade(new BetrayalUpgrade());
        //AddUpgrade(new MortarUpgrade());
        //AddUpgrade(new OnlyHitAllyUpgrade()); 
        //AddUpgrade(new AttackDelayUpgrade()); 
        //AddUpgrade(new RetalliateUpgrade()); 
        //AddUpgrade(new MomentumUpgrade()); 
        //AddUpgrade(new FragileUpgrade()); 
    }


    public void AddUpgrade(BlockUpgrade upgrade)
    {
        blockUpgrades.Add(upgrade);
        upgrade.BlockController = blockController;

        // I am not using a switch statement becuase I am planning for upgrades to be able to be multiple types, so every one needs to be checked per upgrade

        if (upgrade is ITurnStartUpgrade turnStartUpgrade)
        {
            turnStartUpgrades.Add(turnStartUpgrade);

            turnStartUpgrades = turnStartUpgrades // This sorts the list by priority bracket, then by block ID (within brackets)
                .OrderByDescending(u => u.turnStartBehaviourPriority)
                .ThenBy(u => (u as BlockUpgrade)?.UpgradeID ?? int.MaxValue) // the extra is needed as the UpgradeID is in the base class, not the Interface
                .ToList();
        }

        // this was copied/altered from the turnstart code above
        if (upgrade is ITurnEndUpgrade turnEndUpgrade)
        {
            turnEndUpgrades.Add(turnEndUpgrade);

            turnEndUpgrades = turnEndUpgrades // This sorts the list by priority bracket, then by block ID (within brackets)
                .OrderByDescending(u => u.turnEndBehaviourPriority)
                .ThenBy(u => (u as BlockUpgrade)?.UpgradeID ?? int.MaxValue) // the extra is needed as the UpgradeID is in the base class, not the Interface
                .ToList();
        }


        if (upgrade is IPlaceUpgrade placeUpgrade)
        {
            placeUpgrades.Add(placeUpgrade);

            placeUpgrades = placeUpgrades // This sorts the list by priority bracket, then by block ID (within brackets)
                .OrderByDescending(u => u.placeBehaviourPriority)
                .ThenBy(u => (u as BlockUpgrade)?.UpgradeID ?? int.MaxValue) // the extra is needed as the UpgradeID is in the base class, not the Interface
                .ToList();
        }

        if (upgrade is ITargetReplacementUpgrade tarRepUpgrade) // This is no longer a subset of target upgrades, as it it functionally different
        {
            targetReplacementUpgrade = tarRepUpgrade; // set this as the new targetReplacementUpgrade
        }

        if (upgrade is ITargetUpgrade targetUpgrade) // currently extra work is needed for target upgrades as target replacements are a subset of targetUpgrades
        {           
            targetUpgrades.Add(targetUpgrade);
        }


        if (upgrade is IAttackUpgrade attackUpgrade)
        {
            attackUpgrades.Add(attackUpgrade);
        }


        if ( upgrade is IAttackConditionUpgrade attackConditionUpgrade)
        {
            attackConditionUpgrades.Add(attackConditionUpgrade);
        }

        if (upgrade is IOnHitUpgrade onHitUpgrade)
        {
            onHitUpgrades.Add(onHitUpgrade);
        }

        if (upgrade is IGetHitUpgrade getHitUpgrade)
        {
            getHitUpgrades.Add(getHitUpgrade);
        }

    }


    public void RemoveUpgrade(BlockUpgrade upgrade)
    {
        if (!blockUpgrades.Contains(upgrade)) // if the upgrade is not in the list of upgrades then return
        {
            return;
        }

        blockUpgrades.Remove(upgrade);

        // Remove from specific upgrade type lists

        if (upgrade is ITurnStartUpgrade turnStartUpgrade)
        {
            turnStartUpgrades.Remove(turnStartUpgrade);
        }
        if (upgrade is ITurnEndUpgrade turnEndUpgrade)
        {
            turnEndUpgrades.Remove(turnEndUpgrade);
        }


        if (upgrade is IPlaceUpgrade placeUpgrade)
        {
            placeUpgrades.Remove(placeUpgrade);
        }

        if (upgrade is ITargetReplacementUpgrade tarRepUpgrade) 
        {
            targetReplacementUpgrade = null; 
        }

        if (upgrade is ITargetUpgrade targetUpgrade)
        {
            targetUpgrades.Remove(targetUpgrade);
        }

        if (upgrade is IAttackUpgrade attackUpgrade)
        {
            attackUpgrades.Remove(attackUpgrade);
        }


        if (upgrade is IAttackConditionUpgrade attackConditionUpgrade)
        {
            attackConditionUpgrades.Remove(attackConditionUpgrade);
        }

        if (upgrade is IOnHitUpgrade onHitUpgrade)
        {
            onHitUpgrades.Remove(onHitUpgrade);
        }

        if (upgrade is IGetHitUpgrade getHitUpgrade)
        {
            getHitUpgrades.Remove(getHitUpgrade);
        }
    }

    /// <summary>
    /// Removes all upgrades from main and sub lists
    /// </summary>
    public void RemoveAllUpgrades()
    {
        blockUpgrades.Clear();

        turnStartUpgrades.Clear();
        turnEndUpgrades.Clear();
        placeUpgrades.Clear();
        targetReplacementUpgrade = null;
        targetUpgrades.Clear();
        attackUpgrades.Clear();
        attackConditionUpgrades.Clear();
        onHitUpgrades.Clear();
        getHitUpgrades.Clear();

    }



    public void CheckTurnStartUpgrades(GameTypes.Turn turn)
    {
        foreach(ITurnStartUpgrade turnStartUpgrade in turnStartUpgrades.ToList()) // the .ToList() creates a copy so that any changes to the original list will not prevent this from running, this allows for upgrades to be deleted, but still counted the final time
        {
            if (turnStartUpgrade.isCurrentlyTrackingTurnStart)
            {
                turnStartUpgrade.TurnStartBehaviour(turn);
            }
            
        }
    }

    public void CheckTurnEndUpgrades(GameTypes.Turn turn)
    {
        foreach (ITurnEndUpgrade turnEndUpgrade in turnEndUpgrades.ToList())
        {
            if (turnEndUpgrade.isCurrentlyTrackingTurnEnd)
            {
                turnEndUpgrade.TurnEndBehaviour(turn);
            }

        }
    }


    /// <summary>
    /// Returns true if the block can proceed to targeting
    /// </summary>
    /// <returns></returns>
    public bool CheckPlaceUpgrades()
    {
        bool preventInitialTargeting = false; //this is the invers of what will be returned

        foreach(IPlaceUpgrade placeUpgrade in placeUpgrades.ToList())
        {
            if (placeUpgrade.preventInitialTargeting)
            {
                preventInitialTargeting = true;
            }

            placeUpgrade.PlaceBehaviour();

        }

        return !preventInitialTargeting;

    }

    public void CheckTargetReplacementUpgrades() // Atm this is only checks target replacement
    {
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
            return;
        }
        else
        {
            print("There is an attack upgrade"); // figure this part out
        }

    }

    /// <summary>
    /// Returns true if all attack conditons within the upgrades have been met
    /// </summary>
    /// <param name="targetBlockController"></param>
    /// <returns></returns>
    public bool CheckAttackConditionUpgrades(BlockController targetBlockController) // I may choose to separate atack conditions so that they arte not a subset of attackUpgrades as i suspect they will be handled completly differently
    {

        foreach (IAttackConditionUpgrade conditionUpgrade in attackConditionUpgrades.ToList())
        {

            //print(conditionUpgrade.OverwriteAllyCheck);
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


    public void CheckOnHitUpgrades(bool didCapture, int attackPower, GameTypes.DirectionEnum attackDir, BlockController defender)
    {
        foreach (IOnHitUpgrade onHitUpgrade in onHitUpgrades.ToList())
        {
            onHitUpgrade.OnHitBehaviour(didCapture, attackPower, attackDir, defender);
        }
    }


    public void CheckGetHitUpgrades(bool isBeforeCapture, bool isCaptured, int attackPower, GameTypes.DirectionEnum defendDir, BlockController attacker)
    {
        foreach (IGetHitUpgrade getHitUpgrade in getHitUpgrades.ToList())
        {
            if(isBeforeCapture == getHitUpgrade.isBeforeCapture) //if the bool passed into hte function matches when the upgrade says it should apply then run the upgrade
            {
                getHitUpgrade.GetHitBehaviour(isCaptured, attackPower, defendDir, attacker);
            }
        }
    }

}
