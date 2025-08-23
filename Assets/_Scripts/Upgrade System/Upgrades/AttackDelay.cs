using UnityEngine;

public class AttackDelay: BlockUpgrade, IPlaceUpgrade
{

    public override int UpgradeID { get; } = 1;

    public int placeBehaviourPriority { get; private set; } = 0;
    public bool preventInitialTargeting { get; private set; } = true;



    private bool subscribed;
    public int turnDelay = 1;
    private int remainingTurns;
    GameTypes.Turn initialTurn; //this keeps track of whose turn the block should ativate on

    // This upgrade causes the block to wait n turns before targeting and then attacking.
    // The block will attack at the start of the turn, before you can make an action. for this purpose waiting 1 turn will cause it to activate on the players next turn. this may change when i standardise turn/round terminology


    private void SubscribeToTurnChange() 
    {
        if (subscribed) return;
        TurnManager.Instance.OnTurnChanged += CheckTurnsRemaining;
        subscribed = true;
    }

    private void UnsubscribeFromTurnChange()
    {
        if (!subscribed) return;
        TurnManager.Instance.OnTurnChanged -= CheckTurnsRemaining;
        subscribed = false;
    }


    public void PlaceBehaviour()
    {
        Debug.Log("Attack Delay");
        remainingTurns = turnDelay;
        initialTurn = TurnManager.Instance.CurrentTurn;

        // start checking for turn change
        SubscribeToTurnChange();

    }

  

    void CheckTurnsRemaining(GameTypes.Turn turn)
    {

        if(initialTurn == turn)
        {
            remainingTurns--;

            if(remainingTurns <= 0)
            {
                UnsubscribeFromTurnChange();
                BlockController.Target();  // by default when the delay is over, it will continue to targeting (followed by attacking)
            }
        }
    }
    

}
