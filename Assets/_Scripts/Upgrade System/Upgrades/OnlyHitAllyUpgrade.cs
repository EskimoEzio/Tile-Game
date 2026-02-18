using UnityEngine;

public class OnlyHitAllyUpgrade : BlockUpgrade, IAttackConditionUpgrade
{
    public override int UpgradeID => 0;

    public bool OverwriteAllyCheck { get; set; } = true;
    public bool OverwriteNilPowerCheck { get; set; } = false;


    public bool CheckCanAttack(BlockController attackingBlock, BlockController targetBlock)
    {

        if(attackingBlock.blockProperties.CurrentTeam == targetBlock.blockProperties.CurrentTeam)
        {
            //Debug.Log("tried to hit ally");
            return true;
        }
        else
        {
            //Debug.Log("tried to hit enjemy");
            return false;
        }


    }


}
