using UnityEngine;

public class OnlyHitAllyUpgrade : BlockUpgrade, IAttackConditionUpgrade
{
    public bool OverwriteAllyCheck { get; set; } = true;
    public bool OverwriteNilPowerCheck { get; set; } = false;


    public bool CheckCanAttack(BlockController attackingBlock, BlockController targetBlock)
    {

        if(attackingBlock.CurrentTeam == targetBlock.CurrentTeam)
        {
            Debug.Log("tried to hit ally");
            return true;
        }
        else
        {
            Debug.Log("tried to hit enjemy");
            return false;
        }


    }


}
