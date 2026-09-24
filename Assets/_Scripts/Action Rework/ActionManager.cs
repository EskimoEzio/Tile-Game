using UnityEngine;
using System.Collections.Generic;
public class ActionManager
{
    




    void AttackAction(BlockController attacker)
    {
        //Create Attack infor and fill it with side attack info
        AttackInfo attackInfo = new AttackInfo(attacker);
        attackInfo.SideAttacks = PrepareAllSideAttacks(attackInfo); //this could have been done in the constiuctor by altering it slightly, but i like the seperation 

        // Resolve all side attacks

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



        #region Search
        // If there is a replacement do that instead, else do default
        // This does not currnetly support dynamic/conditional changes to attackrange, it only uses the static range
        for (int i = 0; i < attackInfo.Attacker.BlockProperties.AttackRange; i++)
        {
            Vector2 directionOffset = direction.ToVector2() * (i+1); // +1 as it has to start with directionVec * 1 not 0
            Vector2 targetLocation = (Vector2)attackInfo.Attacker.BlockProperties.transform.position + directionOffset;

            if (!GridManager.Instance.Tiles.ContainsKey(targetLocation)) // if the grid does NOT contain the key
            {
                continue;
            }
            if(GridManager.Instance.Tiles[targetLocation] != null)
            {
                targets.Add(new TargetInfo(GridManager.Instance.Tiles[targetLocation]));
            }

        }
        #endregion


        #region Eligibility
        // This is only a default check for of the tile contains a block (it should as currently onlyblocks can be added to the targets list) and is the target a different team, upgrades will be able to modify/ stack on this

        List<TargetInfo> targetsToRemove = new List<TargetInfo>();

        foreach (TargetInfo targetInfo in targets)
        {
            BlockController targetBlockController;

            if(targetInfo.TargetTile.TileContents.TryGetComponent<BlockController>(out BlockController blockController)) //checking for properties here, but this will probably be changed to controller at some point
            {
                targetBlockController = blockController;
            }
            else
            {
                //remove this target as it does not have a block controller - is this the best way?
                targetsToRemove.Add(targetInfo);
                continue;
            }

            if(attackInfo.Attacker.BlockProperties.CurrentTeam == targetBlockController.BlockProperties.CurrentTeam)
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
    #endregion



#region Attack Resolution


    void ResolveAllSideAttacks(AttackInfo attackInfo)
    {

        foreach(SideAttackInfo sideAttack in attackInfo.SideAttacks)
        {
            HitSequence(attackInfo, sideAttack);
        }

    }

    void HitSequence(AttackInfo attack, SideAttackInfo sideAttack)
    {


    }



#endregion

}
