using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))] // require a collider2d as this is important for knowing if the mouse is over the block
public class BlockController : MonoBehaviour
{

    //public event Action<BlockController> OnTeamChanged;
    //public static event Action<BlockController> OnAnyBlockPlaced;
    //public static event Action<BlockController> OnAnyBlockTarget; - i don't know if i need a target event, this may only be necessary if i decude to use events to handle the upgrades
    //public static event Action<BlockController> OnAnyBlockAttack;
    //public static event Action<BlockController> OnAnyBlockGetHit; 
    //public event Action<BlockController> OnThisBlockPlaced;
    //public event Action<BlockController> OnThisBlockTarget;
    //public event Action<BlockController> OnThisBlockAttack;
    //public event Action<BlockController> OnThisBlockGetHit; 

    #region Fields/Variables
    private UpgradeManager upgradeManager;

    [SerializeField] private GameObject spikeHolder;
    private SpikeManager spikeManager;

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



    private List<(BlockController target, Vector2 direction)> targetsAndDirections = new();  //this has to be a list of tuples, becuase i may eventually want to have functionality which which would require non unique keys which cant be don in a dictionary. 


    //STATS
    public int attackRange { get; private set; } = 1;
    #endregion


    private void Awake()
    {
        blockRenderer = GetComponent<SpriteRenderer>();
        artHolderRenderer = artHolderObject.GetComponent<SpriteRenderer>();
        BlockData = defaultData;
        upgradeManager = GetComponent<UpgradeManager>();
        spikeManager = spikeHolder.GetComponent<SpikeManager>();
        

    }

    private void Start()
    {
        InitialiseBlock(null, CurrentTeam);
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
            { GameTypes.Directions[0],  BlockData.PowerValues[0] },
            { GameTypes.Directions[1], BlockData.PowerValues[1] },
            { GameTypes.Directions[2], BlockData.PowerValues[2] },
            { GameTypes.Directions[3], BlockData.PowerValues[3] }
        };

        artHolderRenderer.sprite = BlockData.Sprite;

        SetBlockColour();
    }


    public void PlaceBlock(Vector2 tilePos)
    {
        IsPlaced = true;

        CoroutineRegistry.RunAndTrack(this, PlaceBehaviour(), true); // this handles shaking, place upgrades and and moves to targeting
        
    }

    IEnumerator PlaceBehaviour() //Place shake, check place upgrades and move to targeting
    {

        float shakeDuration = 0.2f;
        float magnitude = 0.1f;
        Vector2 originalPos = transform.localPosition;
        float timeElapsed = 0f;

        while (timeElapsed < shakeDuration)
        {
            timeElapsed += Time.deltaTime;

            // Calculate how far we can shake (decreases over time)
            float percentComplete = timeElapsed / shakeDuration;
            float damper = 1.0f - percentComplete;

            // Move to a random spot within a circle
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude * damper;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude * damper;

            transform.localPosition = originalPos + new Vector2(x, y);

            yield return null;
        }

        transform.localPosition = originalPos;

        // --- PLACE UPGRADES AND TARGETING ---
        if (upgradeManager.CheckPlaceUpgrades()) // if all upgrades allow continuing targeting
        {
            Target();
        }

    }


    public void Target() // This is the targeting step, it will be triggered by an event
    {
        upgradeManager.CheckTargetUpgrades(); // if there is a replacement upgrade, do that instead
        Attack();
    }


    private void Attack() // This is the function that deicdes if each target should get hit or not
    {
        upgradeManager.CheckAttackUpgrades(); // upgrades that should happen just before the attack

        foreach ((BlockController target, Vector2 direction) targetAndDir in targetsAndDirections)
        {

            if (!upgradeManager.CheckAttackConditionUpgrades(targetAndDir.target)) // if it fails the upgrade can attack checks
            {
 
                continue;
            }

            // Default Checks - these are handled differently to the replacement of targeting, becuse they are very simple and there are very few of them
            if(!upgradeManager.OverwriteBaseAllyCheck && CurrentTeam == targetAndDir.target.CurrentTeam) // if the ally check has not been overwritten & if they are on the same team, continue
            {
                continue;
            }

            int power = PowerDict[targetAndDir.direction]; //power is onyl important at this point

            if (!upgradeManager.OverwriteBaseNilPowerCheck && power == 0) // if the NilPower check has not been overwritten and if power is 0. This prevents sides with 0 power from hitting blocks
            {
                continue;
            }
            spikeManager.SpikeAttackEffect();

            targetAndDir.target.BaseGetHit(targetAndDir.direction * -1, power); // This calls the "get hit" function on the target block. The target is the one that decides if it gets captured. This could maybe ue used later to trigger events

        }


    }


    public void BaseTarget() // this resets the targetsAndDirections list then adds the targets (the adjacent blocks within attack range)
    {
        print("base target");
        targetsAndDirections = new(); //this has to be a list of tuples, becuase i may eventually want to have functionality which which would require non unique keys which cant be don in a dictionary. 
        foreach (Vector2 direction in GameTypes.Directions) // this is for checking attacks in every direction
        {


            Vector2 targetLocation = (Vector2)transform.position + direction;
            bool targetAquired = false;

            for(int i = 0; i < attackRange; i++) //if it finds a non-empty tile within range, make that the new target location 
            {
                targetLocation += direction * i;
                if (!GridManager.Instance.Tiles.ContainsKey(targetLocation)) 
                {
                    continue;
                }

                if (GridManager.Instance.Tiles[targetLocation].TileContents != null)
                {
                    targetAquired = true;
                    break;
                }
            }


            if (!targetAquired) // if no targets were found in range check next direction
            {
                continue;
            }


            if (GridManager.Instance.Tiles[targetLocation].TileContents.TryGetComponent<BlockController>(out BlockController targetBlockController))
            { 

                targetsAndDirections.Add((targetBlockController, direction));

            }
        }

    }


    public void BaseAttack(List<(BlockController target, Vector2 direction)> targetsAndDirections)
    {

        foreach((BlockController target, Vector2 direction) targetAndDir in targetsAndDirections)
        {
            int power = PowerDict[targetAndDir.direction];

            if(power == 0)
            {
                continue;
            }

            targetAndDir.target.BaseGetHit(targetAndDir.direction * -1, power); // This calls the "get hit" function on the target block. The target is the one that decides if it gets captured. This could maybe ue used later to trigger events

        }
    }


    private void BaseHit() // this handles each individual hit, the attack function applies this to every target
    {

    }


    /// <summary>
    /// This returns true if the targeted block was captured. 
    /// </summary>
    /// <param name="defendingDir">This is the direction of the defending block, usually the opposite to the attack direction</param>
    /// <param name="attackPower">The Power of the attack</param>
    /// <returns></returns>
    public bool BaseGetHit(Vector2 defendingDir, int attackPower) //I am not certain i want this to return a value, I will have to think about this a bit more
    {
        if (attackPower > PowerDict[defendingDir])
        {
            GetCaptured(defendingDir);
            return true; //the block was captured
        }
        else
        {
            // Not captured (the hit failed)
            return false;
        }
    }




    #region Team Management

    /// <summary>
    /// This changes the blocks current team & flips it
    /// </summary>
    public void ChangeTeam(Vector2 defendingDir = default) //this is currently called to make the enemy's blocks on the correct team, this may have to be changed as it is broadcasting events that may be needed elsewhere
    {
        
        if(defendingDir == default)
        {
            defendingDir = Vector2.down;
        }

        CurrentTeam = GameUtilities.ToggleTeam(CurrentTeam);

        CoroutineRegistry.RunAndTrack(this, BlockFlip(defendingDir), true);

    }

    /// <summary>
    /// Captures this block, changing its team & flips it
    /// </summary>
    /// <param name="defendingDir">The direction the of the defending side</param>
    public void GetCaptured(Vector2 defendingDir) // Is this function necessary now? I may just be able to use the chage team funciton
    {
        // as it is now, becuase the blocks are 2D and have no depth, you cannot distinguish flipping left or right, however i am making it change so that when i later switch to a 3d block it will be easier
        // Trigger onCaptured event

        ChangeTeam(defendingDir);

    }

    /// <summary>
    /// This handles the flip animation & colourchange when a block changes team. It should not be called directly.
    /// </summary>
    /// <param name="defDir">The defending direction</param>
    /// <returns></returns>
    IEnumerator BlockFlip(Vector2 defDir)
    {

        Vector2 axis = -Vector2.Perpendicular(-defDir);

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

        SetBlockColour();

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
            spikeManager.SetSpikeRowColour(PlayerColour);
        }
        else
        {
            blockRenderer.color = EnemyColour;
            spikeManager.SetSpikeRowColour(EnemyColour);
        }
    }
    #endregion

}
