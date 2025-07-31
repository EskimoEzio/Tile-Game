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

    private void Start()
    {
        if (IsMultiplayer) // All of the special things that need to be done if it is multiplayer
        {
            //Disable Enemy manager as it is not needed in multiplayer
            FindAnyObjectByType<EnemyManager>().gameObject.SetActive(false);

            //Create the Enemy Hand
            GameObject enemyHand = Instantiate(FindAnyObjectByType<HandManager>().gameObject); // I may be able to combine this and the folling line, but i have seperated for clarity
            HandManager enemyHandManager = enemyHand.GetComponent<HandManager>();
            enemyHandManager.handTeam = GameTypes.Team.Enemy;
        }
    }



    private void EndTurn(GameTypes.Turn turn)
    {
        (int playerScore, int enemyScore) = CalcScores();

        if (IsMultiplayer)
        {
            //print("Blue: " + playerScore + ", Red: " + enemyScore);
        }
        else
        {
            print("PLAYER: " + playerScore + ", ENEMY: " + enemyScore);

        }


        

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
                print("Blue Wins With a Score of " + playerScore);
            }
            else if (enemyScore > playerScore)
            {
                print("Red Wins With a Score of " + enemyScore);
            }
            else
            {
                print("It's a Tie!!");
            }

            ShowLuck();
        }
        else //Singleplayer
        {
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
                if(blockController.CurrentTeam == GameTypes.Team.Player)
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

 
    private void ShowLuck() // this is not necessary at all, it is just a fun function to calc approximate luck
    {
        int playerLuck = 0;
        int enemyLuck = 0;


        List<HandManager> hands = new List<HandManager>(FindObjectsByType<HandManager>(FindObjectsSortMode.None));

        foreach (HandManager hand in hands)
        {

            if(hand.handTeam == GameTypes.Team.Player)
            {
                playerLuck = hand.luck;
            }
            else if (hand.handTeam == GameTypes.Team.Enemy)
            {
                enemyLuck = hand.luck;
            }
        }
        
        //print("Blue Luck: " + playerLuck + " Enemy Luck: " + enemyLuck);

        float luckPercentage = 0f;
        if (playerLuck > enemyLuck)
        {
            luckPercentage = ((float)playerLuck / (float)enemyLuck) - 1f;
            luckPercentage = Mathf.Round(luckPercentage * 100);

            print("Blue's Luck was " + luckPercentage + "% Better");
        }
        else if(enemyLuck > playerLuck)
        {
            luckPercentage = ((float)playerLuck / (float)enemyLuck) - 1f;
            luckPercentage = Mathf.Round(luckPercentage * 100);
            print("Red's's Luck was " + luckPercentage + "% Better");
        }
        else
        {
            print("No Luck Involved!");
        }


    }

}
