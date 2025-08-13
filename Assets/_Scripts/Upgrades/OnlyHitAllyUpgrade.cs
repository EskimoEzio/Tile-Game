using UnityEngine;

public class OnlyHitAllyUpgrade : IAttackConditionUpgrade
{
    public bool OverwriteAllyCheck { get; set; } = true;
    public bool OverwriteNilPowerCheck { get; set; } = true;


    public bool CheckCanAttack(BlockController attackingBlock, BlockController targetBlock)
    {

        if(attackingBlock.CurrentTeam == targetBlock.CurrentTeam)
        {
            return true;
        }
        else
        {
            return false;
        }


    }


}
