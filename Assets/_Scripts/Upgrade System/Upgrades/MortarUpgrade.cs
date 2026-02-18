using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MortarUpgrade: BlockUpgrade, ITargetReplacementUpgrade
{
    public override int UpgradeID => 2;

    // this does not change the range by itself, i want this to be handled i nthe future stat system

    public void Target(BlockController blockController)
    {
        
        List<(BlockController target, GameTypes.DirectionEnum direction)> targetsAndDirections = new(); //this has to be a list of tuples, becuase i may eventually want to have functionality which which would require non unique keys which cant be don in a dictionary. 

        foreach (GameTypes.DirectionEnum direction in GameTypes.AllDirections) // this is for checking attacks in every direction
        {

            Vector2 targetLocation = (Vector2)blockController.gameObject.transform.position + direction.ToVector2() * blockController.blockProperties.AttackRange; // target location is exactly attack range away

            if (!GridManager.Instance.Tiles.ContainsKey(targetLocation)) //if tile doesnt exist check next direction
            {
                continue;
            }

            if (GridManager.Instance.Tiles[targetLocation].TileContents == null)
            {
                continue;
            }



            if (GridManager.Instance.Tiles[targetLocation].TileContents.TryGetComponent<BlockController>(out BlockController targetBlockController))
            {
                if (targetBlockController.blockProperties.CurrentTeam == blockController.blockProperties.CurrentTeam) // if the defending block is on the same team as the attacking block, then do not try to attack
                {
                    continue;
                }


                targetsAndDirections.Add((targetBlockController, direction));

            }
        }

        // Start Attack if there are targets
        if (targetsAndDirections.Count != 0)
        {
            blockController.Attack(targetsAndDirections); // fix this once I change how the whole attack process works
        }
    }

}
