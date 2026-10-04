using UnityEngine;
using System.Collections.Generic;
public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance;

    private void Awake()
    {
        Instance = this;
    }



    public void AttackAction(BlockController attacker)
    {
        //Create Attack infor and fill it with side attack info
        AttackInfo attackInfo = new AttackInfo(attacker);
        attackInfo.SideAttacks = PrepareAllSideAttacks(attackInfo); //this could have been done in the constiuctor by altering it slightly, but i like the seperation 

        // Resolve all side attacks

        PerformAllSideAttacks(attackInfo);
        ResolveAllHitResults(attackInfo);

        // Resolve post hit effects

        // Resolve post attack reactions

    }

#region Attack Preparation
    List<SideAttackInfo> PrepareAllSideAttacks(AttackInfo attackInfo)
    {

        List<SideAttackInfo> sideAttacks = new List<SideAttackInfo>();

        foreach(GameTypes.DirectionEnum direction in GameTypes.AllDirections)
        {
            sideAttacks.Add(new SideAttackInfo(direction, SideTarget(attackInfo, direction))); // this adds a new side attack to the list with a list of targets for the given direction
        }

        return sideAttacks;
    }


    List<TargetInfo> SideTarget(AttackInfo attackInfo, GameTypes.DirectionEnum direction)
    {
        List<TargetInfo> targets = new List<TargetInfo>();
        //int modifiedRange = attackInfo.Attacker.BlockProperties.AttackRange; //by default it is the default range, it is only different if upgrades modify it below

        SearchParameters searchParameters = new SearchParameters(attackInfo.Attacker.BlockProperties.AttackRange);

        ApplySearchModifyUpgrades(attackInfo, searchParameters, direction);


        #region Search
        // If there is a replacement do that instead, else do default (below)
        // This does not currnetly support dynamic/conditional changes to attackrange, it only uses the static range

        if(attackInfo.Attacker.UpgradeManager.GetSearchReplacementUpgrade() != null)
        {
            targets = attackInfo.Attacker.UpgradeManager.GetSearchReplacementUpgrade().Search(attackInfo, direction, searchParameters);
        }
        else
        {
            targets = DefaultSearch(attackInfo, direction, searchParameters);
        }

        

        #endregion


        #region Eligibility
        // This is only a default check for of the tile contains a block (it should as currently onlyblocks should have reached this point) and is the target a different team, upgrades will be able to modify/ stack on this

        List<TargetInfo> targetsToRemove = new List<TargetInfo>();



        foreach (TargetInfo targetInfo in targets)
        {
            BlockController targetBlockController;
            bool isEligible = true;

            if(targetInfo.TargetTile.TileContents.TryGetComponent<BlockController>(out BlockController blockController)) //checking for properties here, but this will probably be changed to controller at some point
            {
                targetBlockController = blockController;
            }
            else
            {
                //remove this target as it does not have a block controller - is this the best way?
                targetsToRemove.Add(targetInfo);
                isEligible = false;
                continue;
            }

            if(attackInfo.Attacker.BlockProperties.CurrentTeam == targetBlockController.BlockProperties.CurrentTeam)
            {
                targetsToRemove.Add(targetInfo);
                isEligible = false;
            }

            ApplyEligibilityUpgrades(attackInfo, targetInfo, direction, ref isEligible);

            if (!isEligible)
            {
                targetsToRemove.Add(targetInfo);
            }
        }



        foreach(TargetInfo invalidTarget in targetsToRemove)
        {
            targets.Remove(invalidTarget);
        }


        #endregion
        
        
        // Selection - this only becomes relevant with upgrades that allow for targeting.hitting more than 1 block (not relevant for defaut so will figure it out later)


        // Modifiers - this only becomes relevant with upgrades that change the targets, by default this will od nothing



        return targets;
    }

    List<TargetInfo> DefaultSearch(AttackInfo attackInfo, GameTypes.DirectionEnum direction, SearchParameters searchParameters)
    {

        List<TargetInfo> targets = new List<TargetInfo>();

        for (int i = 0; i < searchParameters.Range; i++)
        {
            Vector2 directionOffset = direction.ToVector2() * (i + 1); // +1 as it has to start with directionVec + 1 not 0
            Vector2 targetLocation = (Vector2)attackInfo.Attacker.BlockProperties.transform.position + directionOffset;

            if (!GridManager.Instance.Tiles.ContainsKey(targetLocation)) // if the grid does NOT contain the key
            {
                continue;
            }
            if (GridManager.Instance.Tiles[targetLocation].TileContents != null) // if the tile is note empty
            {
                //if(searchParameters.AlliesBlock)
                
                
                targets.Add(new TargetInfo(GridManager.Instance.Tiles[targetLocation]));

                if (searchParameters.StopAtFirstOccupied)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }

        }

        return targets;
    }

    void ApplySearchModifyUpgrades(AttackInfo attackInfo, SearchParameters searchParameters, GameTypes.DirectionEnum direction)
    {

        List<ISearchModifierUpgrade> searchModifierUpgrades = attackInfo.Attacker.upgradeManager.GetSearchModifierUpgrades();
        foreach(ISearchModifierUpgrade searchModifierUpgrade in searchModifierUpgrades)
        {
            searchModifierUpgrade.ModifySearch(searchParameters, direction);
        }
    }

    void ApplyEligibilityUpgrades(AttackInfo attackInfo, TargetInfo targetInfo, GameTypes.DirectionEnum direction, ref bool isElgibile)
    {
        List<IEligibilityUpgrade> eligibilityUpgrades = attackInfo.Attacker.upgradeManager.GetEligibilityUpgrades();

        foreach(IEligibilityUpgrade eligibilityUpgrade in eligibilityUpgrades)
        {
            eligibilityUpgrade.ModifyEligibility(attackInfo, targetInfo, direction, ref isElgibile);
        }
    }

    #endregion



    #region Attack Resolution


    void PerformAllSideAttacks(AttackInfo attackInfo)
    {

        foreach(SideAttackInfo sideAttack in attackInfo.SideAttacks) //calculate the hits and queue the outcomes
        {
            HitSequence(attackInfo, sideAttack);
        }
    }

    void ResolveAllHitResults(AttackInfo attackInfo)
    {

        foreach (HitResolutionInfo hitResolutionInfo in attackInfo.PendingHitResults)
        {
            // do i need to chekc for hitOccurred?
            if (hitResolutionInfo.HitResultInfo.HitOccurred == false)
            {
                continue;
            }

            switch (hitResolutionInfo.HitResultInfo.FinalOutcome)
            {
                case HitOutcome.NoChange:
                    break;
                case HitOutcome.Captured:
                    hitResolutionInfo.HitInfo.Target.ChangeTeam(hitResolutionInfo.HitInfo.AttackDirection.Invert());
                    break;
                case HitOutcome.Broken:
                    hitResolutionInfo.HitInfo.Target.GetBroken();
                    break;
            }


        }
    }

    void HitSequence(AttackInfo attackInfo, SideAttackInfo sideAttackInfo)
    {

        foreach(TargetInfo targetInfo in sideAttackInfo.Targets)
        {
            if (targetInfo.TargetTile.TileContents == null)
            {
                continue;
            }

            // get the blockController on the target
            BlockController target;
            if(targetInfo.TargetTile.TileContents.TryGetComponent<BlockController>(out BlockController blockController))
            {
                target = blockController;
            }
            else
            {
                continue; // if there is no block controller, move to the next targetInfo
            }

            HitInfo hitInfo = new HitInfo(attackInfo.Attacker, target, sideAttackInfo.Direction, attackInfo.Attacker.BlockProperties.PowerDict[sideAttackInfo.Direction]);

            HitResultInfo hitResultInfo =  Hit(hitInfo);

            attackInfo.PendingHitResults.Add(new HitResolutionInfo(hitInfo, hitResultInfo));

        }

    }


    HitResultInfo Hit(HitInfo hitInfo)
    {
        HitResultInfo hitResultInfo = new HitResultInfo(false, HitOutcome.NoChange); //default values for teh hitResult

        // Check Hit Conditions
        // this will be further changed through upgrades, similar to serach eligibility
        //defaul team check is below

        if (hitInfo.Attacker.BlockProperties.CurrentTeam == hitInfo.Target.BlockProperties.CurrentTeam)
        {
            hitResultInfo.FinalOutcome = HitOutcome.NoChange;
            hitResultInfo.HitOccurred = false;
            return hitResultInfo;
        }
        

        // Apply Hit Effects
        // this is based on upgreades

        hitResultInfo =  GetHit(hitInfo);

        //

        return hitResultInfo;
    }


    HitResultInfo GetHit(HitInfo hitInfo)
    {
        HitResultInfo hitResultInfo = new HitResultInfo(true, HitOutcome.NoChange);

        // Immediate Get Hit responses

        // Calculate base result - using default power check
        if (hitInfo.CurrentPower > hitInfo.Target.BlockProperties.PowerDict[hitInfo.AttackDirection.Invert()])
        {
            hitResultInfo.FinalOutcome = HitOutcome.Captured;

        }else
        {
            hitResultInfo.FinalOutcome = HitOutcome.NoChange;
        }

        // Result Get Hit responses

        // Determine final result

        // Resolve final result

        return hitResultInfo;
    }

    #endregion

}
