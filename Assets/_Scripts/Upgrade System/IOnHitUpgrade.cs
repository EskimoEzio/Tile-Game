using UnityEngine;

public interface IOnHitUpgrade
{
    int onHitBehaviourPriority { get; }

    void OnHitBehaviour(bool didCapture, int attackPower, Vector2 attackDir, BlockController defender);
}
