using UnityEngine;

public class MomentumUpgrade : BlockUpgrade, ITurnStartUpgrade, IStatUpgrade
{

    public override int UpgradeID { get; } = 6;

    public int turnStartBehaviourPriority { get; private set; } = 0;
    public bool isCurrentlyTrackingTurnStart { get; set; } = true;

    int powerIncrease = 1;
    int numberOfIncreases = 2; // this means that it will pick a random side and add 1 power, then repeat 2 times

    

    public void TurnStartBehaviour(GameTypes.Turn turn)
    {
        if (!BlockController.blockProperties.IsPlaced) return; //if the block is not placed then skip, this will be changed when i change how block states work
        
        if(GameUtilities.CheckTurnMatchTeam(turn, BlockController.blockProperties.CurrentTeam)) 
        {
            ApplyStatUpgrade(BlockController.blockProperties);
        }
    }

    public void ApplyStatUpgrade(BlockProperties blockProperties)
    {
        for(int i = 0; i<numberOfIncreases; i++)
        {
            blockProperties.ModifyPower(powerIncrease, 1);
        }
    }

}
