using System;
using UnityEngine;

[Serializable]
public class AttackPowerIncreaseUpgrade : BlockUpgrade, IHitModifierUpgrade
{
    public override int UpgradeID { get; } = 15;

    [SerializeField] private int additiveIncrease;
    [SerializeField] private int multiplier = 1;

    public void ModifyHit(HitInfo hitInfo)
    {
        hitInfo.CurrentPower += additiveIncrease;
        hitInfo.CurrentPower *= multiplier;

    }
}
