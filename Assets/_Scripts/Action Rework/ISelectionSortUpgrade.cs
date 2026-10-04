using UnityEngine;
using System.Collections.Generic;

public interface ISelectionSortUpgrade
{
    void SortTargets(AttackInfo attackInfo, GameTypes.DirectionEnum direction, List<TargetInfo> targets);
}
