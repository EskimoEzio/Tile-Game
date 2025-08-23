using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MortarUpgrade: BlockUpgrade, ITargetReplacementUpgrade
{
    public override int UpgradeID => 2;

    // this will have to increase the range, by at least 1 and then will directly attack that tile - to do this i need to implement the stat system

    public void Target(BlockController blockController)
    {
        

        List<Vector2> directions = new List<Vector2> { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

        List<(BlockController target, Vector2 direction)> targetsAndDirections = new(); //this has to be a list of tuples, becuase i may eventually want to have functionality which which would require non unique keys which cant be don in a dictionary. 

        foreach (Vector2 direction in directions) // this is for checking attacks in every direction
        {


            Vector2 targetLocation = (Vector2)blockController.gameObject.transform.position + direction;

            targetLocation += direction * blockController.attackRange; // target location is exactly attack range away
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
                if (targetBlockController.CurrentTeam == blockController.CurrentTeam) // if the defending block is on the same team as the attacking block, then do not try to attack
                {
                    continue;
                }


                targetsAndDirections.Add((targetBlockController, direction));

            }
        }

        // Start Attack if there are targets
        if (targetsAndDirections.Count != 0)
        {
            blockController.BaseAttack(targetsAndDirections); // fix this once I change how the whole attack process works
        }
    }

}
