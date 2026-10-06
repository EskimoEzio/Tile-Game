using System;
using UnityEngine;

[Serializable]
public class DeathTouchUpgrade : BlockUpgrade, IHitOutcomeModifierUpgrade
{
    public override int UpgradeID { get; } = 17;

    public bool applyOnAttack => true;

    public bool applyOnDefend => false;

    public void ModifyHitOutcome(ReceiveHitContext receiveHitContext, HitResultInfo hitResultInfo)
    {
        if(hitResultInfo.HitOccurred == true) //only if the hit actually occurred, is this necessary?
        {
            hitResultInfo.FinalOutcome = HitOutcome.Broken;
        }
    }
}
