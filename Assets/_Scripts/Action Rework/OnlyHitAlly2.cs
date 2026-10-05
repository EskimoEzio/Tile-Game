using System;
using UnityEngine;

[Serializable]
public class OnlyHitAlly2 : BlockUpgrade, IEligibilityUpgrade, IHitConditionUpgrade // this will also need to be a hitcondition upgrade
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

    public void ModifyHitCondtion(HitInfo hitInfo, ref bool canHit)
    {
        if (hitInfo.Attacker.BlockProperties.CurrentTeam == hitInfo.Defender.BlockProperties.CurrentTeam)
        {
            canHit = true;
        }
        else //if the teams do not match then it cannot hit
        {
            canHit = false;
        }
    }
}
