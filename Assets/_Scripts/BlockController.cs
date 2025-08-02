using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))] // require a collider2d as this is important for knowing if the mouse is over the block
public class BlockController : MonoBehaviour
{

    public event Action<BlockController> OnTeamChanged;


    public event Action<BlockController> OnBlockPlaced;
    public event Action<BlockController> OnAttack; // the actions that happen when the block "attacks" - not necessarily damage - i think this will usually just be trigger OnHits - The Attack windup??
    public event Action<BlockController, BlockController> OnHit; //when the attack hits what does it do (deal damage, buff/debuff etc.)
    //public event Action<BlockController> OnGetHit; //when hit by an attack how does this block respond - this event may not be necessary as i can just have two parameters in the OnHit event



    public BlockData BlockData { get; private set; }
    [SerializeField] private BlockData defaultData;

    private SpriteRenderer blockRenderer;
    [SerializeField] private GameObject spikeObject;

    [SerializeField] private GameObject artHolderObject;
    private SpriteRenderer artHolderRenderer;


    public Dictionary<Vector2, int> PowerDict { get; private set; } // this dictionary holds the power of each block in a given direction, represented by a Vector2 of that direction

    public GameTypes.Team CurrentTeam { get; private set; }

    public bool IsPlaced { get; private set; }

    public Color EnemyColour = Color.red;
    public Color PlayerColour = Color.blue;

    [SerializeField] private float flipSpeed = 360f;


    private void Awake()
    {
        blockRenderer = GetComponent<SpriteRenderer>();
        artHolderRenderer = artHolderObject.GetComponent<SpriteRenderer>();
        BlockData = defaultData;

        InitialiseBlock();

    }

    /// <summary>
    /// Set up the block & assign it's team.
    /// </summary>
    /// <param name="newData"></param>
    /// <param name="team"></param>
    public void InitialiseBlock(BlockData newData = null, GameTypes.Team team = GameTypes.Team.Player)
    {
        if(newData != null)
        {
            BlockData = newData;
        }

        CurrentTeam = team;

        // Assign the power values based on the values in the BockData scriptable object
        PowerDict = new Dictionary<Vector2, int>()
        {
            { Vector2.up,  BlockData.PowerValues[0] },
            { Vector2.right, BlockData.PowerValues[1] },
            { Vector2.down, BlockData.PowerValues[2] },
            { Vector2.left, BlockData.PowerValues[3] }
        };

        artHolderRenderer.sprite = BlockData.Sprite;

        SetBlockColour();
    }


    private void BaseBlockPlaced()
    {

        //at this point the block has already been added to the tile. By default the next thing is targeting. No extra work required
        IsPlaced = true;

        OnAttack?.Invoke(this); // Will is still use these events? probably, but this will be moved from here (BlockPlace)
    }

    private void BaseTarget() // by default look at all sides with spikes, return a list of any adjacent enemy blocks 
    {

        List<(BlockController target,Vector2 direction)> targetsAndDirections = new(); //this has to be a list of tuples, becuase i may eventually want to have functionality which which would require non unique keys which cant be don in a dictionary. 

        foreach (KeyValuePair<Vector2, int> dirPow in PowerDict) // this is for checking attacks in every direction
        {

            if (dirPow.Value == 0) // no attack if there is no power on that side
            {
                continue;
            }

            Vector2 targetLocation = (Vector2)transform.position + dirPow.Key;

            if (!GridManager.Instance.Tiles.ContainsKey(targetLocation) || GridManager.Instance.Tiles[targetLocation].TileContents == null) //if tile doesnt exist || or is empty
            {
                continue;
            }


            if (GridManager.Instance.Tiles[targetLocation].TileContents.TryGetComponent<BlockController>(out BlockController targetBlockController))
            { 
                if (targetBlockController.CurrentTeam == CurrentTeam) // if the defending block is on the same team as the attacking block, then do not try to attack
                {
                    continue;
                }


                targetsAndDirections.Add((targetBlockController, dirPow.Key));

            }
        }

        // START ATTACK
        BaseAttack(targetsAndDirections);

    }


    private void BaseAttack(List<(BlockController target, Vector2 direction)> targetsAndDirections)
    {

        foreach((BlockController target, Vector2 direction) targetAndDir in targetsAndDirections)
        {
            int power = PowerDict[targetAndDir.direction];

            if(power == 0)
            {
                continue;
            }

            if (power > targetAndDir.target.PowerDict[targetAndDir.direction * -1])
            {
                targetAndDir.target.GetCaptured(targetAndDir.direction); //capture if power is higher
            }


        }

    }



    public void PlaceBlock(Vector2 tilePos)
    {
        IsPlaced = true;

        OnBlockPlaced?.Invoke(this);


        //this is the default targeting, adjacent, orthogonal, range 1

        foreach(KeyValuePair<Vector2, int> dirPow in PowerDict) // this is for checking attacks in every direction
        {

            if(dirPow.Value != 0)
            {
                Attack(tilePos, dirPow.Key, dirPow.Value);
            }
            
        }


    }

    private void Attack(Vector2 tilePos, Vector2 dir, int power)
    {
        // check if the tiles dict (in grid manager) does not contain the current tile + offset (if it exists)
        if (!GridManager.Instance.Tiles.ContainsKey(tilePos + dir))
        {
            //print(tilePos + dirPow.Key + ": does not exist");
            return;
        }

        // check if the tile is empty
        if (GridManager.Instance.Tiles[tilePos + dir].TileContents == null)
        {
            //print(tilePos + dirPow.Key + ": is empty");
            return;
        }

        // if the tile contains a gameobject with block controller then proceed with checks
        if (GridManager.Instance.Tiles[tilePos + dir].TileContents.TryGetComponent<BlockController>(out BlockController targetBlockController))
        {
            // if the defending block is on the same team as the attacking block, then do not try to attack
            if (targetBlockController.CurrentTeam == CurrentTeam)
            {
                //print(targetBlockController.BlockData.BlockName + " is on the same team");
                return;
            }

            // this is the check of the power of new block in dir, compared to pow of old block in opposite dir (new block attacks up, old bolck defends down)
            if (power > targetBlockController.PowerDict[dir * -1]) //this is set so that ties do not count as wins
            {
                //print(BlockData.BlockName + " beats " + targetBlockController.BlockData.BlockName);
                //targetBlockController.ChangeTeam();
                targetBlockController.GetCaptured(dir);
            }
            else if (power < targetBlockController.PowerDict[dir * -1])
            {
                //print(gameObject.name + " loses to " + targetBlockController.gameObject.name);
            }
            else
            {
                //print("It's a Tie");
            }

        }
    }



    /// <summary>
    /// This changes the blocks current team. It should not be called directly
    /// </summary>
    public void ChangeTeam() //this is currently called to make the enemy's blocks on the correct team, this may have to be changed as it is broadcasting events that may be needed elsewhere
    {
        CurrentTeam = GameUtilities.ToggleTeam(CurrentTeam);
        
        SetBlockColour();
        // do anything else related to changing team
        OnTeamChanged?.Invoke(this); //this broadcasts that this blockController has changed team, with the new team as a parameter

    }

    /// <summary>
    /// Captures this block, changing its team and flipping it
    /// </summary>
    /// <param name="attackDir">The direction the of the attack</param>
    public void GetCaptured(Vector2 attackDir)
    {
        // as it is now, becuase the blocks are 2D and have no depth, you cannot distinguish flipping left or right, however i am making it change so that when i later switch to a 3d block it will be easier

        Vector2 rotationAxis = -Vector2.Perpendicular(attackDir);

        //StartCoroutine(BlockFlip(rotationAxis));
        CoroutineRegistry.RunAndTrack(this, BlockFlip(rotationAxis), true);
    }

    IEnumerator BlockFlip(Vector2 axis)
    {
        float amountRotated = 0f;
        
        while (amountRotated < 90f)
        {
            float frameRotationAmount = Mathf.Min(flipSpeed * Time.deltaTime, 90 - amountRotated);
            transform.Rotate(axis, frameRotationAmount);
            amountRotated += frameRotationAmount;

            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, 0); // set rotation to 0 temporarily to makehte next step easier
        transform.Rotate(axis, -90); // the amount is minus, because i am getting it ready for the second portion, by flipping it halfway, so that i only have to do 180 total
        
        ChangeTeam();

        amountRotated = 0;

        while (amountRotated < 90f)
        {
            float frameRotationAmount = Mathf.Min(flipSpeed * Time.deltaTime, 90 - amountRotated);
            transform.Rotate(axis, frameRotationAmount);
            amountRotated += frameRotationAmount;

            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, 0); // finally ensure that the rotation is completly reset
    }

 
    

    private void SetBlockColour()
    {
        if (CurrentTeam == GameTypes.Team.Player)
        {
            blockRenderer.color = PlayerColour;
        }
        else
        {
            blockRenderer.color = EnemyColour;
        }
    }


}
