using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandManager : MonoBehaviour
{
    [SerializeField] private List<BlockData> library = new List<BlockData>();
    [SerializeField] private GameObject blockPrefab;

    public GameTypes.Team handTeam; //This is the team that the hand belongs to

    private List<GameObject> hand = new List<GameObject>();
    private BoxCollider2D handCollider;
    private int maxHandSize = 3;
    [SerializeField] private float blockGap = 0.1f;
    [SerializeField] private float gridHandGap = 0.3f;
    

    Camera mainCam;

    [SerializeField] private int startingHandSize = 3;


    private void OnEnable()
    {
        TurnManager.Instance.OnTurnChanged += PlayerTurn;
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnTurnChanged -= PlayerTurn;
    }



    private void Awake()
    {
        handCollider = GetComponent<BoxCollider2D>();

        mainCam = Camera.main;
        
    }


    private void Start()
    {
        PositionHand();
        DrawBlock(startingHandSize);
    }

    void PositionHand()
    {
        float xPos = (float)GridManager.Instance.gridWidth / 2 - 0.5f;

        float yPos = 0;

        if(handTeam == GameTypes.Team.Player)
        {
            yPos = -1 - gridHandGap;
        }
        else if(handTeam == GameTypes.Team.Enemy)
        {
            yPos = GridManager.Instance.gridHeight + 0 + gridHandGap;
        }
        else
        {
            Debug.LogError("Hands team is neither player nor enemy");
        }


        //This sets the hand to the correct x position as well as y position. SO far this only works for the players hand not the enemy
        transform.position = new Vector2(xPos, yPos);
        
    }


    void PlayerTurn(GameTypes.Turn turn)
    {
        
        if(!GameUtilities.CheckTurnMatchTeam(turn, handTeam)) // if the turn does not match the hand's team
        {
            return;
        }

        if (hand.Count < maxHandSize)
        {
            DrawBlock(1);
        }
    }



    public void AddToHand(GameObject block, Vector2 mouseScreenPos = default)
    {
        if(hand.Count >= maxHandSize)
        {
            // hand is full - figure out what i will actually do in the scenario
            print("Hand is Full!");
        }

        int targetIndex = hand.Count;

        Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseScreenPos);
        if (handCollider.OverlapPoint(mouseWorldPos)) // only rearrange hand if mouse is over the han collider. Otherwise add the block to the end
        {
            // if the mouse is in the hand collider (may need to make this cover the entire bottom of the screen) then loop through the list to find the first block that the mouse is to the left of, then set the target index to that block's index
            for(int i = 0; i < hand.Count; i++)
            {
                
                if (mouseWorldPos.x <= hand[i].transform.position.x)
                {
                    //print("insert at: " + i);
                    targetIndex = i;
                    break;
                }
            }

        }

        // insert the block at the target index
        hand.Insert(Mathf.Min(hand.Count, targetIndex), block);

        // next is to set up so that cards in hand set their positions correctly. i will need ot know how much space i want between blocks and use this to figure out the postiions for each block. afterwards,m i can look at lerping the movement
        // for now i will just set hte position to the hand
        block.transform.position = new Vector2(transform.position.x, transform.position.y);

        // This is just to help keep track of where each block is, it may end up not being super helpful, since i can just do hand.contains


        SpaceBlocks();

    }

    public void RemoveFromHand(GameObject block)
    {
        hand.Remove(block);


        SpaceBlocks();
    }

    void SpaceBlocks()
    {
        if(hand.Count == 0) // do not need to do this if the hand is empty
        {
            return;
        }

        // all blocks should be the same width, so i will just get the width of the first, this will mean i don't have to update anyhting if i change the size later
        float blockWidth = hand[0].GetComponent<BoxCollider2D>().size.x;
        int numOfGaps = hand.Count - 1;
        float startX = transform.position.x - (((hand.Count-1) * 0.5f * blockWidth) + (numOfGaps * 0.5F * blockGap));

        for(int i = 0; i < hand.Count; i++)
        {
            GameObject block = hand[i];
            float xPos = startX + i * (blockWidth + blockGap);
            block.transform.position = new Vector2(xPos, block.transform.position.y);

        }

    }

    
    void DrawBlock(int quant = 1) // this is a simple draw function that draws a random block each turn, it picks from a list called library, but it is random and does not behave like an actual library
    {


        // draw 3 at the star of the game
        for (int i = 0; i < quant; i++)
        {
            int randIndex = Random.Range(0, library.Count);
 
            GameObject newBlock = Instantiate<GameObject>(blockPrefab, new Vector2(20, 20), Quaternion.identity);
            BlockController blockController = newBlock.GetComponent<BlockController>();

            blockController.InitialiseBlock(library[randIndex], handTeam);

            AddToHand(newBlock);
        }

        
    }


}
