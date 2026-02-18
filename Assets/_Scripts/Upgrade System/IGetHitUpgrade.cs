using UnityEngine;

public interface IGetHitUpgrade
{
    int getHitBehaviourPriority { get; }

    bool isBeforeCapture { get; }

    void GetHitBehaviour(bool isCaptured, int attackPower, GameTypes.DirectionEnum defendDir, BlockController attacker); // what parameters do i want (maybe the same as the on hit)
}
