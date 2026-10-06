using UnityEngine;

public interface IHitOutcomeModifierUpgrade
{
    bool applyOnAttack { get;}
    bool applyOnDefend { get;}
    
    void ModifyHitOutcome(ReceiveHitContext receiveHitContext, HitResultInfo hitResultInfo);
}
