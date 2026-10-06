using System;
using UnityEngine;

[Serializable]
public class Fragile2Upgrade : BlockUpgrade, IHitOutcomeModifierUpgrade
{
    public override int UpgradeID { get; } = 18;

    public bool applyOnAttack => false;

    public bool applyOnDefend => true;


    public void ModifyHitOutcome(ReceiveHitContext receiveHitContext, HitResultInfo hitResultInfo)
    {
        if (hitResultInfo.FinalOutcome == HitOutcome.Captured) 
        {
            hitResultInfo.FinalOutcome = HitOutcome.Broken;
        }
    }
}
