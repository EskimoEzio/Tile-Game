using UnityEngine;

public interface IHitConditionUpgrade
{
    void ModifyHitCondtion(HitInfo hitInfo, ref bool canHit);
}
