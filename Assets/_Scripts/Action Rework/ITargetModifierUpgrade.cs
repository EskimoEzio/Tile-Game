using UnityEngine;
using System.Collections.Generic;

public interface ITargetModifierUpgrade
{
    void ModifyTargets(AttackInfo attackInfo, GameTypes.DirectionEnum direction, List<TargetInfo> targets);
}
