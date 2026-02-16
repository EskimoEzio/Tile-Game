using UnityEngine;

public class TrampleUpgrade : BlockUpgrade, IOnHitUpgrade
{
    public override int UpgradeID => 5;

    public int onHitBehaviourPriority => 0;

    public void OnHitBehaviour(bool didCapture, int attackPower, Vector2 attackDir, BlockController defender)
    {
        if (!didCapture) return; //if it did not capture the one it hit then return

        int excessPower = attackPower - defender.PowerDict[attackDir * -1];

        Debug.Log(defender.BlockData.Sprite.name + " got hit! Excess power: " + excessPower);

        if(excessPower <= 0) return; // if there is no excess power return


        //try target the next tile in the attack direction

        Vector2 targetLocation = (Vector2)defender.transform.position + attackDir;

        if (!GridManager.Instance.Tiles.ContainsKey(targetLocation)) return; // if the tile would fall outside of the grid then return


        if (GridManager.Instance.Tiles[targetLocation].TileContents == null) return; //if the tile is not empty then return


        if (GridManager.Instance.Tiles[targetLocation].TileContents.TryGetComponent<BlockController>(out BlockController targetBlockController))
        {
            BlockController.BaseAttack(targetBlockController, attackDir, excessPower);
        }



    }
}
