using System;
using UnityEngine;

[Serializable]
public class EvolveUpgrade : BlockUpgrade, IPlaceUpgrade, ITurnEndUpgrade
{
    // this changes the current block into another at the end of x turns. thurn delay of 1 means the end of the turn it is placed
    // importantly, this does not count as placing a block nor does it cause a hit, this will be a seperate upgrade

    public override int UpgradeID { get; } = 9;

    public int placeBehaviourPriority { get; private set; } = 0;
    public bool preventInitialTargeting { get; private set; } = false;

    public int turnEndBehaviourPriority { get; private set; } = -5;
    public bool isCurrentlyTrackingTurnEnd { get; set; } = false;

    [SerializeField] private int turnDelay = 2; // this needs to be serialised as it is for configuration


    [SerializeField] private BlockData evolveBlockData;

    private int remainingTurns;

    public void PlaceBehaviour()
    {
        remainingTurns = turnDelay;
        isCurrentlyTrackingTurnEnd = true;
    }

    public void TurnEndBehaviour(GameTypes.Turn turn)
    {
        if (BlockController.BlockProperties.CurrentLocation != GameTypes.BlockLocation.Board) return; //if the block is not on the board then skip

        remainingTurns--;

        if (remainingTurns <= 0)
        {
            //BlockController.BlockProperties.BlockData = evolveBlockData;
            isCurrentlyTrackingTurnEnd = false;
            BlockController.InitialiseBlock(evolveBlockData, BlockController.BlockProperties.CurrentTeam);
            

        }
    }

}
