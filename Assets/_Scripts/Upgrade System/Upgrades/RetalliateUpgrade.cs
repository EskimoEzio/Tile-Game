using UnityEngine;

public class RetalliateUpgrade : BlockUpgrade, IGetHitUpgrade, ITurnStartUpgrade
{
    public override int UpgradeID => 6;
    public int getHitBehaviourPriority => 0;
    public bool isBeforeCapture => false; //this has to happen after capture, as it only retaliates if the capture failed - Iactually think this is incorrect now, as i can pass in the isCaptured bool to determine, this. However, i am leaving it as it is atm, becuase it having it after also seems okay for now

    public int turnStartBehaviourPriority => 0;

    public bool isCurrentlyTrackingTurnStart { get; set; } = true;

    private bool hasRetaliatedThisTurn = false;

    public void GetHitBehaviour(bool isCaptured, int attackPower, GameTypes.DirectionEnum defendDir, BlockController attacker)
    {
        if (hasRetaliatedThisTurn) return; // do not retaliate if you have already retaliated once this turn

        hasRetaliatedThisTurn = true;
        //Debug.Log(BlockController.BlockData.Sprite.name + " got hit at: " + Time.realtimeSinceStartupAsDouble);
        //Debug.Log(BlockController.BlockData.Sprite.name + " retalliates against " + attacker.BlockData.Sprite.name);

        BlockController.BaseAttack(attacker, defendDir); //it runs the attack code which limits what it can hhit back, but also allows it to activate any Attack upgrades

    }

    public void TurnStartBehaviour(GameTypes.Turn turn)
    {
        hasRetaliatedThisTurn = false;
    }
}
