using UnityEngine;

public interface IReceiveHitModifierUpgrade
{
    void ModifyReceiveHit(HitInfo hitInfo, ReceiveHitContext receiveHitContext);
}
