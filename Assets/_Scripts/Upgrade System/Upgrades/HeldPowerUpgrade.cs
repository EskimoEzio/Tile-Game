using System;
using UnityEngine;

[Serializable]
public class HeldPowerUpgrade : BlockUpgrade, ITurnEndUpgrade, IStatUpgrade
{

    // this grants an increase of x whenever you end your turn with it in your hand.
    public override int UpgradeID { get; } = 10;

    public int turnEndBehaviourPriority { get; private set; } = 0;
    public bool isCurrentlyTrackingTurnEnd { get; set; } = true;


    [SerializeField] private int powerIncrease = 1; // amount of power
    [SerializeField] private int numberOfIncreases = 1; // number times to add



    public void TurnEndBehaviour(GameTypes.Turn turn)
    {
        if (BlockController.BlockProperties.CurrentLocation != GameTypes.BlockLocation.Hand) return; //if the block is not in hand

        if(GameUtilities.CheckTurnMatchTeam(turn, BlockController.BlockProperties.CurrentTeam)) // only do a power increase when the turn matches the block's team 
        {
            ApplyStatUpgrade(BlockController.BlockProperties);
        }
    }

    public void ApplyStatUpgrade(BlockProperties blockProperties)
    {
        for (int i = 0; i < numberOfIncreases; i++)
        {
            blockProperties.ModifyPower(powerIncrease, 1);
        }
    }
}
