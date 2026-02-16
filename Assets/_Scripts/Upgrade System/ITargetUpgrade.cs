using UnityEngine;

public interface ITargetUpgrade
{
    //ATM this is a tracker interface that helps to group all target upgrades
    int targetBehaviourPriority { get; }

    void TargetBehaviour();
}
