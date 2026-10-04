using UnityEngine;

public interface ISelectionCountUpgrade
{
    void ModifySelectionCount(AttackInfo attackInfo, GameTypes.DirectionEnum direction, ref int targetCount);
}
