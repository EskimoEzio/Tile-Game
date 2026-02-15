using UnityEngine;

public class BetrayalUpgrade : BlockUpgrade, IPlaceUpgrade, ITurnStartUpgrade
{
    public override int UpgradeID { get; } = 4;

    public int placeBehaviourPriority { get; private set; } = 0;
    public bool preventInitialTargeting { get; private set; } = false;

    public int turnStartBehaviourPriority { get; private set; } = 5; // thus is the highest priority atm, so it will change team before anything else at the start of turn
    public bool isCurrentlyTrackingTurnStart { get; set; } = false;

    public int turnDelay = 2; // this means by default it will change on your next turn
    private int remainingTurns;

    // After n turns, at the start of the turn, change team. 1 means change team at the start of the next turn. This does not inherently cause an attack


    public void PlaceBehaviour()
    {
        remainingTurns = turnDelay;
        isCurrentlyTrackingTurnStart = true;

    }

    public void TurnStartBehaviour(GameTypes.Turn turn)
    {
        remainingTurns--;

        if (remainingTurns <= 0)
        {
            BlockController.ChangeTeam();
            isCurrentlyTrackingTurnStart = false;
        }
    }
}
