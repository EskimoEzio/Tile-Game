using UnityEngine;

public class BetrayalUpgrade : BlockUpgrade, IPlaceUpgrade  
{
    public override int UpgradeID { get; } = 4;

    public int placeBehaviourPriority { get; private set; } = 5;
    public bool preventInitialTargeting { get; private set; } = false;




    // After n turns, change team. 0 means change team instantly. 
    // ATM this onyl does instant change as i need to standardise terminology surrounding turns



    public void PlaceBehaviour() // This is on pause until I change the change team logic
    {

        Debug.Log("Betrayal");


    }


}
