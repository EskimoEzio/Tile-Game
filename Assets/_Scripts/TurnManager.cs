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

    public enum Turn
    {
        Player,
        Enemy
    }

    public Turn CurrentTurn { get; private set; }

    // This event takes a parameter of the new turn - e.g. if the player's turn is ending, the parameter will be enemy turn
    public event Action<Turn> OnTurnChanged;


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

        
        if(CurrentTurn == Turn.Player)
        {
            CurrentTurn = Turn.Enemy;
        }
        else
        {
            CurrentTurn = Turn.Player;
        }
        //print(CurrentTurn + "'s Turn");
        actionsTaken++;
        turnNumber = 1 + actionsTaken / 2;
        //print(turnNumber);

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
