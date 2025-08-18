using UnityEngine;

public class AttackDelay: BlockUpgrade, IPlaceUpgrade
{

    public bool preventInitialTargeting { get; set; } = true;

    private bool subscribed;
    public int turnDelay = 1;
    int remainingTurns;
    GameTypes.Turn initialTurn; //this keeps track of whose turn the block should ativate on

    // This upgrade causes the block to wait n turns before targeting and then attacking.
    // The block will attack at the start of the turn, before you can make an action. for this purpose waiting 1 turn will cause it to activate on the players next turn. this may change when i standardise turn/round terminology


    private void Subscribe()
    {
        if (subscribed) return;
        TurnManager.Instance.OnTurnChanged += CheckTurnsRemaining;
        subscribed = true;
    }

    private void Unsubscribe()
    {
        if (!subscribed) return;
        TurnManager.Instance.OnTurnChanged -= CheckTurnsRemaining;
        subscribed = false;
    }


    public void PlaceBehaviour()
    {

        remainingTurns = turnDelay;
        initialTurn = TurnManager.Instance.CurrentTurn;

        // start checking for turn change
        Subscribe();

    }

  

    void CheckTurnsRemaining(GameTypes.Turn turn)
    {

        if(initialTurn == turn)
        {
            remainingTurns--;

            if(remainingTurns <= 0)
            {
                Unsubscribe();
                blockController.Target();  // by default when the delay is over, it will continue to targeting (followed by attacking)
            }
        }
    }
    

}
