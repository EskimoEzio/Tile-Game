using UnityEngine;

public abstract class BlockUpgrade
{
    // ATM this is a blank base class. It allows me to store all upgrades in a single list <IBlockUpgrade>

    public BlockController BlockController; // this may not need to be public. This will be assigned to the owner block when it is added to the upgrade list.

    public abstract int UpgradeID { get; } // this is an abstract property. It is not defined here and any child must define it. 

}
