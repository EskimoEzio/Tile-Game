using UnityEngine;

public interface IAttackUpgrade
{
    // This is just a marker interface atm

    int attackBehaviourPriority { get; }

    void AttackBehaviour();
}
