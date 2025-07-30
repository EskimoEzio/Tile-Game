using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    // this makes the input handler a singlton, this means that any script can access this class but there can only be one instance (which is fine because i don't need more than 1)
    public static TurnManager Instance;

    public int turnNumber { get; private set; } // this keeps trackk of turn number. a turn is defined as 1 round of the paly + enemy making an action
    private int actionsTaken = 0; // this keeps track of the number of actions performed, its purpose is that int actions/2 is the turn number. This makes for an easy way of keeping track of the number of turns while ignoring if the player or enemy went first


    public GameTypes.Turn CurrentTurn { get; private set; } // this tracks the whose turn it is. It changes between player and enemy in both single and multi player

    public GameTypes.Turn ActivePlayer { get; private set; } // this keeps track of which HUMAN player has control, the second is still referred to as the enemy. In singleplayer, it is always "Player"

    // This event takes a parameter of the new turn - e.g. if the player's turn is ending, the parameter will be enemy turn
    public event Action<GameTypes.Turn> OnTurnChanged;


    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //turnNumber = 1;
        //print(CurrentTurn + "'s Turn");

        ActivePlayer = GameTypes.Turn.Player; // by default the player goes first

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

        
        if(CurrentTurn == GameTypes.Turn.Player)
        {
            CurrentTurn = GameTypes.Turn.Enemy;
        }
        else
        {
            CurrentTurn = GameTypes.Turn.Player;
        }
        print(CurrentTurn + "'s Turn");
        actionsTaken++;
        turnNumber = 1 + actionsTaken / 2;
        //print(turnNumber);

        if (GameManager.Instance.IsMultiplayer) // the active player only changes if there are two players
        {
            ActivePlayer = CurrentTurn;
        }
        
        
        
        OnTurnChanged?.Invoke(CurrentTurn);       
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
