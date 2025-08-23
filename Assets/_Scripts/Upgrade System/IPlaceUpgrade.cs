using UnityEngine;

public interface IPlaceUpgrade
{
    int placeBehaviourPriority { get;} //this determines the order upgrades are executed. +5 => first, -5 => last. There is only "get" because the "private set" will be declared in each upgrade

    bool preventInitialTargeting { get; } //There is only "get" because the "private set" will be declared in each upgrade


    void PlaceBehaviour(); // maybe rename this


}
