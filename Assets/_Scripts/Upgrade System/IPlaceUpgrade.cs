using UnityEngine;

public interface IPlaceUpgrade
{
    // This is just a marker interface atm
    // add a priority system

    bool preventInitialTargeting { get; set; }


    void PlaceBehaviour(); //rename this


}
