using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsMultiplayer;

    private int playerScore;


    private void OnEnable()
    {
        TurnManager.Instance.OnTurnChanged += EndTurn;
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnTurnChanged -= EndTurn;
    }



    private void Awake()
    {
        Instance = this;

    }



    private void EndTurn(TurnManager.Turn turn)
    {
        (int playerScore, int enemyScore) = CalcScores();

        if (IsMultiplayer)
        {
            print("PLAYER 1: " + playerScore + ", ENEMY 2: " + enemyScore);
        }


        print("PLAYER: " + playerScore + ", ENEMY: " + enemyScore);

    }



    /// <summary>
    /// This function checks if the grid is full at the end of a turn. If so the game will end and it will output the winner. This is done before the OnTurnChanged event is broadcast
    /// </summary>
    public bool CheckGameOver()
    {

        foreach(KeyValuePair<Vector2, Tile> posTile in GridManager.Instance.Tiles) 
        {
            if(posTile.Value.TileContents == null) // if there is an empty tile, do not end the game
            {
                return false;
            }
        }

        GameOver();
        return true;
    }

    void GameOver()
    {
        (int playerScore, int enemyScore) = CalcScores();

        if (IsMultiplayer)
        {
            if (playerScore > enemyScore)
            {
                print("Player 1 Wins With a Score of " + playerScore);
            }
            else if (enemyScore > playerScore)
            {
                print("Player 2 Wins With a Score of " + enemyScore);
            }
            else
            {
                print("It's a Tie!");
            }
        }


        if (playerScore > enemyScore)
        {
            print("You Win!!! With a Score of " + playerScore);
        }
        else if (enemyScore > playerScore)
        {
            print("You Lose, With a Score of " + playerScore);
        }
        else
        {
            print("It's a Tie!");
        }
    }


    /// <summary>
    /// This updates the scores at the end of each round and when the game ends
    /// </summary>
    public (int, int) CalcScores()
    {

        int playerScore = 0;
        int enemyScore = 0;

        foreach (KeyValuePair<Vector2, Tile> posTile in GridManager.Instance.Tiles) // this is for checking attacks in every direction
        {
            if(posTile.Value.TileContents == null) // if the tile is empty, skip it
            {
                continue;
            }
            
            if(posTile.Value.TileContents.TryGetComponent<BlockController>(out BlockController blockController))
            {                
                if(blockController.CurrentTeam == BlockController.Team.Player)
                {
                    playerScore++;
                }
                else
                {
                    enemyScore++;
                }
            }
        }

        return (playerScore, enemyScore);
    }

    
}
