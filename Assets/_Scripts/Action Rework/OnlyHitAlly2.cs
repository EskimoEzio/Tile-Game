using System;
using UnityEngine;

[Serializable]
public class OnlyHitAlly2 : BlockUpgrade, IEligibilityUpgrade // this will also need to be a hitcondition upgrade
{
    public override int UpgradeID { get; } = 13;

    public void ModifyEligibility(AttackInfo attackInfo, TargetInfo targetInfo, GameTypes.DirectionEnum direction, ref bool isEligible)
    {
        BlockController targetBlock = targetInfo.TargetTile.TileContents.GetComponent<BlockController>();

        if (attackInfo.Attacker.BlockProperties.CurrentTeam == targetBlock.BlockProperties.CurrentTeam)
        {
            isEligible = true;
        }
        else //if the teams do not match then it cannot hit
        {
            isEligible = false;
        }
    }

}
