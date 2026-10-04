using UnityEngine;

public interface IEligibilityUpgrade
{

    void ModifyEligibility(AttackInfo attackInfo, TargetInfo targetInfo, GameTypes.DirectionEnum direction, ref bool isEligible);

}
