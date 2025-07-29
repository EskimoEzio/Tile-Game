using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{

    [SerializeField] private List<BlockData> hand = new List<BlockData>();

    private BlockData nextBlock; //currently, i am iterating through the list
    private Dictionary<Vector2, int> PowerDict;
    private BlockController nextBlockController;
    [SerializeField] private GameObject blockPrefab;
    private GameObject blockObject;
    private Vector2 spawnLocation = new Vector2(20, 20); // this is just an offscreen location, it is arbitrary

    [SerializeField] private float prePlaceDelay = 0.6f;
    [SerializeField] private float postPlaceDelay = 0.1f;

    // SCORING VALUES
    [SerializeField] private float baseCaptureValue = 100;
    [SerializeField] private float captureStrengthBonus = 0.2f;
    [SerializeField] private float overKillPenalty = 0.1f;

    [SerializeField] private float noWastedPowerBonus = 50f;
    [SerializeField] private float wastedPowerLinearPenalty = 5f;
    [SerializeField] private float wastedPowerExponentialPenalty = 25f;



    private void OnEnable()
    {
        TurnManager.Instance.OnTurnChanged += StartEnemyTurn;
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnTurnChanged -= StartEnemyTurn;
    }



    void StartEnemyTurn(TurnManager.Turn turn)
    {

        if (turn == TurnManager.Turn.Enemy)
        {
            //StartCoroutine(EnemyTurn(turn));
            CoroutineRegistry.RunAndTrack(this, EnemyTurn(turn));
        }
        
    }


    IEnumerator EnemyTurn(TurnManager.Turn turn)
    {
        

        ChooseBlock(); // select the block that will be placed

        Vector2 targetPos = ChooseTile(); // select where the block wil be placed

        yield return new WaitForSeconds(prePlaceDelay);

        GridManager.Instance.Tiles[targetPos].gameObject.GetComponent<Tile>().AddToTile(blockObject); // access the Tile component and add the block to the target tile

        yield return new WaitForSeconds(postPlaceDelay);

        TurnManager.Instance.EndTurn();

    }




    private void ChooseBlock() 
    {
        nextBlock = hand[(TurnManager.Instance.turnNumber - 1) % hand.Count]; // this uses a fixed order. I can easily make it a random order of the current hand, but will leave it like this for now

        blockObject = Instantiate(blockPrefab, spawnLocation, Quaternion.identity);        
        nextBlockController = blockObject.GetComponent<BlockController>();
        
        nextBlockController.InitialiseBlock(nextBlock);
        nextBlockController.ChangeTeam(); //this is currently called to make the enemy's blocks on the correct team, this may have to be changed


        PowerDict = new Dictionary<Vector2, int>()
        {
            { Vector2.up,  nextBlock.PowerValues[0] },
            { Vector2.right, nextBlock.PowerValues[1] },
            { Vector2.down, nextBlock.PowerValues[2] },
            { Vector2.left, nextBlock.PowerValues[3] }
        };
    }

    private Vector2 ChooseTile() // I will add a lot more to this, including looking at position and block combination
    {
        List<Vector2> bestTiles = new List<Vector2>();
        float bestScore = -1_000_000f; // i have set this to -1 mil so that the first tile will always be considered a best tile
    

        Dictionary<Vector2, Tile> emptyTiles = new Dictionary<Vector2, Tile>();

        foreach(KeyValuePair<Vector2, Tile> tile in GridManager.Instance.Tiles)
        {
            if(tile.Value.TileContents == null)
            {
                emptyTiles.Add(tile.Key, tile.Value);
            }
        }

        //evaluate all empty tiles - each tile will get a score based on what it can capture, and where its remaining power is

        foreach(KeyValuePair<Vector2, Tile> tile in emptyTiles)
        {
            float currentScore = EvaluateTile(nextBlock, tile.Key);

            if(currentScore > bestScore)
            {
                bestScore = currentScore;
                bestTiles.Clear();
                bestTiles.Add(tile.Key);
                //print(bestScore);
            }
            else if(currentScore == bestScore)
            {
                bestTiles.Add(tile.Key);
            }
        }
        // pick a random option from the bestTiles list
        //print("bestTiles.Count = " + bestTiles.Count);
        if (bestTiles.Count == 1)
        {
            return bestTiles[0];
        }
        else if (bestTiles.Count == 0)
        {
            print("No Tiles Left");
            enabled = false; //this shouldn't be necessary as i am now checking before the enemy's turn if there are any tiles left
        }

        int randi = Random.Range(0, bestTiles.Count);
        Vector2 selectedTile = bestTiles[randi];
        //print(selectedTile);

        //print(bestTiles);

        return selectedTile;

    }


    private float EvaluateTile(BlockData BlockData, Vector2 tilePos)
    {
        float score = 0;
        //int wastedPower = 0; //this just keeps track of any power that is not utilised. This will be minimised


        // first work out how many tiles it could potentially caputure
        foreach (KeyValuePair<Vector2, int> dirPow in PowerDict)
        {
            if (!GridManager.Instance.Tiles.ContainsKey(tilePos + dirPow.Key)) // if the direction has no tile, the power that way is wasted
            {
                score += CalcWastedPowerScore(dirPow.Value);
                continue;
            }

            if (GridManager.Instance.Tiles[tilePos + dirPow.Key].TileContents == null) // if the direction is empty, the power is not going to waste as it defends, that is why i do not increase wasted power here
            {
                continue;
            }


            // if the tile contains a gameobject with block controller then proceed with checks, i have done this the other way so that i can make use of TryGetComponents "out" (i am checking for block controller atm, as this is the script with the power values)
            if (GridManager.Instance.Tiles[tilePos + dirPow.Key].TileContents.TryGetComponent<BlockController>(out BlockController targetBlockController))
            {

                // if the defending block is on the same team as the attacking block, then do not try to attack
                if (targetBlockController.CurrentTeam == BlockController.Team.Enemy)
                {
                    score += CalcWastedPowerScore(dirPow.Value);
                    continue;
                }

                if (dirPow.Value > targetBlockController.PowerDict[dirPow.Key * -1]) //if it wins in the given directon
                {
                    //print(gameObject.name + " beats " + targetBlockController.gameObject.name);
                    //score += BaseCaptureValue * (1+(CaptureStrengthBonus* targetBlockController.PowerDict[dirPow.Key * -1]));

                    score += CalcCaptureScore(dirPow.Value, targetBlockController.PowerDict[dirPow.Key * -1]);


                }
                else // does not capture
                {
                    score += CalcWastedPowerScore(dirPow.Value);

                }

            }

        }


        //print(tilePos + " : " + score);
        return score;
    }


    // Score Functions - these are WIP they will change once the game works to try to actually balance

    private float CalcCaptureScore(int attackingPower, int defendingPower)
    {
        return baseCaptureValue * (1 + captureStrengthBonus * defendingPower - overKillPenalty * (attackingPower-1 - defendingPower));
    }

    private float CalcWastedPowerScore(int wastedPower)
    {

        if (wastedPower == 0)
        {
            return noWastedPowerBonus;
        }
        else
        {
            float wastedPowerFloat = wastedPower;
            return - wastedPowerLinearPenalty * wastedPowerFloat - Mathf.Pow(wastedPowerExponentialPenalty, (1 + wastedPowerFloat / 10));
        }
        
        //print("wasted score: " + (noWastedPowerBonus - wastedPowerLinearPenalty * wastedPower - Mathf.Pow(wastedPowerExponentialPenalty, (1 + wastedPower / 10))));
        //return noWastedPowerBonus - wastedPowerLinearPenalty * wastedPower - Mathf.Pow(wastedPowerExponentialPenalty, (1 + wastedPower / 10));
    }

}
