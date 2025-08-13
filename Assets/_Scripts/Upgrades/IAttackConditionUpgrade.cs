using UnityEngine;

public interface IAttackConditionUpgrade : IBlockUpgrade
{
    // These are upgrades that affect if a block should attack a target block

    bool OverwriteAllyCheck { get; set; }
    bool OverwriteNilPowerCheck { get; set; }


    public bool CheckCanAttack(BlockController attackingBlock, BlockController targetBlock);

}
