using UnityEngine;

public class BetrayalUpgrade : BlockUpgrade, IPlaceUpgrade  
{
    public override int UpgradeID { get; } = 4;

    public int placeBehaviourPriority { get; private set; } = 5;
    public bool preventInitialTargeting { get; private set; } = false;


    private bool subscribed;
    public int turnDelay = 2; // this means by default it will change on your next turn
    private int remainingTurns;

    // After n turns, at the end of the turn, change team. 0 means change team at the end of the current turn 
    // ATM this onyl does instant change as i need to standardise terminology surrounding turns

    private void Subscribe()
    {
        if (subscribed) return;
        TurnManager.Instance.OnTurnEnded += CheckTurnsRemaining;
        subscribed = true;
    }
    private void Unsubscribe()
    {
        if (!subscribed) return;
        TurnManager.Instance.OnTurnEnded -= CheckTurnsRemaining;
        subscribed = false;
    }


    public void PlaceBehaviour() // This is on pause until I change the change team logic
    {
        remainingTurns = turnDelay;

        // start checking for turn change
        Subscribe();
    }

    

    void CheckTurnsRemaining(GameTypes.Turn turn)
    {

        remainingTurns--;

        if (remainingTurns <= 0)
        {
            Unsubscribe();
            BlockController.ChangeTeam(); // when delay is over, change team (do not attack)
        }

    }


}
