using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    // this makes the input handler a singlton, this means that any script can access this class but there can only be one instance (which is fine because i don't need more than 1)
    public static TurnManager Instance;


    public int ActionCount { get; private set; } // This is the number of moves made by a player on their turn. this does not work ATM as I have not implemented the ability to make more than 1 action on a turn
    public int TurnNumber { get; private set; } = 1; // This keeps track of turn number. A turn encompasses all actions made by a single player. It ends when a player recahes their maxuimum actions (not yet implemented)
    public int RoundNumber { get; private set; } // This keeps track of round number. A round is completed after both players have 1 turn. This currently does not accout for if a player has multiple turns in a row or i someone msises a turn. it simply divides turnnumber by 2 (player count)

    //private int actionsTaken = 0; // this keeps track of the number of actions performed, its purpose is that int actions/2 is the turn number. This makes for an easy way of keeping track of the number of turns while ignoring if the player or enemy went first


    public GameTypes.Turn CurrentTurn { get; private set; } // this tracks the whose turn it is. It changes between player and enemy in both single and multi player
    public GameTypes.Turn NextTurn { get; private set; } // this tracks whose turn comes next, by default it is the alternative to CurrentTurn

    public GameTypes.Turn ActivePlayer { get; private set; } // this keeps track of which HUMAN player has control, the second is still referred to as the enemy. In singleplayer, it is always "Player"


    public event Action<GameTypes.Turn> OnTurnChanged; // Parameter of new turn  - This keeps track of when the turn changes between player and enemy - It is not necessary atm, but could be helpful for some upgrades later
    public event Action<GameTypes.Turn> OnTurnEnded; // Parameter of the turn that ended
    public event Action<GameTypes.Turn> OnTurnStarted; // Parameter of the turn that started
    // On Turn Changed will be changed so that it only triggers when it swaps from payer => enemy (or vice versa). This event may not be used anymore


    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ActivePlayer = GameTypes.Turn.Player; // by default the player goes first

    }


    public void CompleteAction() // May rename this - ATM it doees not have anywhere to look for the number of actions per turn so it is just set to 1
    {
        int maxActions = 1; // in the future this will reference the number of actions the player is able to make instead of being fixed

        ActionCount++;

        if(ActionCount == maxActions)
        {
            EndTurn();
        }


    }


    public void EndTurn()
    {
        if (GameManager.Instance.CheckGameOver())
        {
            return;
        }

        if (CoroutineRegistry.CheckEndTurnBlocked())
        {
            CoroutineRegistry.RunAndTrack(this, DelayEndTurn());
            return;
        }

        /*
        if(CurrentTurn == GameTypes.Turn.Player)
        {
            CurrentTurn = GameTypes.Turn.Enemy;
        }
        else
        {
            CurrentTurn = GameTypes.Turn.Player;
        }*/

        
        OnTurnEnded?.Invoke(CurrentTurn);
        
        

        if(CurrentTurn != NextTurn) //this may get changed to be after StartTurn. It is not necessary atm, but could be helpful for some upgrades later
        {
            OnTurnChanged?.Invoke(NextTurn);
        }

        StartTurn(NextTurn);

    }

    /// <summary>
    /// This begins the next turn
    /// </summary>
    /// <param name="turn">The turn that is starting</param>
    void StartTurn(GameTypes.Turn turn)
    {
        TurnNumber++;
        CurrentTurn = turn;
        NextTurn = GameUtilities.ToggleTurn(turn); //by default the next players turn is the opposite of this players turn - this may change (allowing for multiple turns in a row

        print(CurrentTurn + "'s Turn");

        OnTurnStarted?.Invoke(NextTurn);

        if (GameManager.Instance.IsMultiplayer) // the active player only changes if there are two players
        {
            ActivePlayer = CurrentTurn;
        }


    }


    private IEnumerator DelayEndTurn()
    {
        while (CoroutineRegistry.CheckEndTurnBlocked())
        {
            yield return null;
        }

        EndTurn();
    }

}
