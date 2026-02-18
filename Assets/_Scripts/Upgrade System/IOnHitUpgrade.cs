using UnityEngine;

public interface IOnHitUpgrade
{
    int onHitBehaviourPriority { get; }

    void OnHitBehaviour(bool didCapture, int attackPower, GameTypes.DirectionEnum attackDir, BlockController defender);
}
