using UnityEngine;

public class AttackDelay: BlockUpgrade, IPlaceUpgrade
{

    public override int UpgradeID { get; } = 1;

    public int placeBehaviourPriority { get; private set; } = 0;
    public bool preventInitialTargeting { get; private set; } = true;



    private bool subscribed;
    public int turnDelay = 1;
    private int remainingTurns;

    // This upgrade causes the block to wait n turns before targeting and then attacking.
    // The block will attack at the start of the turn, before you can make an action. for this purpose waiting 1 turn will cause it to activate at the start of the enemies turn. waiting 2 will cause it to attack at the start of players next turn


    private void Subscribe() 
    {
        if (subscribed) return;
        TurnManager.Instance.OnTurnStarted += CheckTurnsRemaining;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed) return;
        TurnManager.Instance.OnTurnStarted -= CheckTurnsRemaining;
        subscribed = false;
    }


    public void PlaceBehaviour()
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
            BlockController.Target();  // by default when the delay is over, it will continue to targeting (followed by attacking)
        }

    }
    

}
