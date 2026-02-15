using UnityEngine;

public interface ITurnStartUpgrade
{
    int turnStartBehaviourPriority { get; } //this determines the order upgrades are executed. +5 => first, -5 => last. There is only "get" because the "private set" will be declared in each upgrade

    // bool IsSubscribedToTurnStart { get; set; }
    bool isCurrentlyTrackingTurnStart { get; set; }

    // may add a bool to check specify whose turn, but may be easier to put in thr behaviour functions

    void TurnStartBehaviour(GameTypes.Turn turn);

    // void SubscribeTurnStart();
    // void UnsubscribeTurnStart();

}
