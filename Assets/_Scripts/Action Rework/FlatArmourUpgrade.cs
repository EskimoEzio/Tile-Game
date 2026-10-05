using System;
using UnityEngine;

[Serializable]
public class FlatArmourUpgrade : BlockUpgrade, IReceiveHitModifierUpgrade
{
    public override int UpgradeID { get; } = 16;

    [SerializeField] private int armourValue = 1;

    public void ModifyReceiveHit(HitInfo hitInfo, ReceiveHitContext receiveHitContext)
    {
        receiveHitContext.DefenderPower += armourValue;
    }
}
