using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))] // require a collider2d as this is important for knowing if the mouse is over the block
[RequireComponent(typeof(BlockProperties))]
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
    public BlockProperties BlockProperties { get; private set; }

    [SerializeField] private GameObject spikeHolder;
    public SpikeManager spikeManager { get; private set; }

    [Header("Stats")]
    //public BlockData BlockData { get; private set; }
    [SerializeField] private BlockData defaultData;

    private SpriteRenderer blockRenderer;
    [SerializeField] private GameObject spikeObject;

    [SerializeField] private GameObject artHolderObject;
    private SpriteRenderer artHolderRenderer;


    //public Dictionary<Vector2, int> PowerDict { get; private set; } // this dictionary holds the power of each block in a given direction, represented by a Vector2 of that direction

    //public GameTypes.Team CurrentTeam { get; private set; }

    public Color EnemyColour = Color.red;
    public Color PlayerColour = Color.blue;

    [SerializeField] private float flipSpeed = 360f;


    /// <summary>
    /// This is a list of the targeted blocks and the side that will attack it
    /// </summary>
    private List<(BlockController target, GameTypes.DirectionEnum direction)> targetsAndDirections = new();  //this has to be a list of tuples, becuase i may eventually want to have functionality which which would require non unique keys which cant be don in a dictionary. 


    //STATS
    //public int attackRange { get; private set; } = 1;
    #endregion


    private void Awake()
    {
        blockRenderer = GetComponent<SpriteRenderer>();
        artHolderRenderer = artHolderObject.GetComponent<SpriteRenderer>();
        //BlockData = defaultData;
        upgradeManager = GetComponent<UpgradeManager>();
        BlockProperties = GetComponent<BlockProperties>();
        spikeManager = spikeHolder.GetComponent<SpikeManager>();
        
    }

    private void Start()
    {
        //InitialiseBlock(null, blockProperties.CurrentTeam); //this is not needed atm, as I initialise blocks when creating them
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
            BlockProperties.BlockData = newData;
        }

        BlockProperties.CurrentTeam = team;
        BlockProperties.InitialiseStats();

        artHolderRenderer.sprite = BlockProperties.BlockData.Sprite;

        SetBlockColour();
    }


    public void PlaceBlock(Vector2 tilePos)
    {
        CoroutineRegistry.RunAndTrack(this, PlaceBehaviour(), true); // this handles shaking, place upgrades and and moves to targeting
        
    }

    IEnumerator PlaceBehaviour() //Place shake, check place upgrades and move to targeting
    {

        float shakeDuration = 0.15f;
        float magnitude = 0.08f;
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
        upgradeManager.CheckTargetReplacementUpgrades(); // if there is a replacement upgrade, do that instead
        Attack(targetsAndDirections);
    }

    /// <summary>
    /// This is the function that starts the attack process
    /// </summary>
    public void Attack(List<(BlockController target, GameTypes.DirectionEnum direction)> tarsAndDirs) // This is the function that deicdes if each target should get hit or not
    {
        foreach ((BlockController target, GameTypes.DirectionEnum direction) targetAndDir in tarsAndDirs)
        {
            BaseAttack(targetAndDir.target, targetAndDir.direction);
        }
    }


    public void BaseTarget() // this resets the targetsAndDirections list then adds the targets (the adjacent blocks within attack range)
    {
        targetsAndDirections = new(); //this has to be a list of tuples, becuase i may eventually want to have functionality which which would require non unique keys which cant be don in a dictionary. 
        foreach (GameTypes.DirectionEnum direction in GameTypes.AllDirections) // this is for checking attacks in every direction
        {
            Vector2 vectorDirection = direction.ToVector2(); // storing it in memory to avoid calling the function repeatedly

            Vector2 targetLocation = (Vector2)transform.position + vectorDirection;
            bool targetAquired = false;

            for(int i = 0; i < BlockProperties.AttackRange; i++) //if it finds a non-empty tile within range, make that the new target location 
            {
                targetLocation += vectorDirection * i;
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



    /// <summary>
    /// This handles a single attack in a given direction
    /// </summary>
    /// <param name="target">The target block</param>
    /// <param name="direction">The attack direction</param>
    /// <param name="presetPower">If the attack should use a specific power instead of teh usual PowDict value</param>
    public void BaseAttack(BlockController target, GameTypes.DirectionEnum direction, int? presetPower = null)
    {
        BlockProperties targetBlockProperties = target.BlockProperties; //get a reference to the block attributes at this point, as may be important for attack upgrades later
        
        if (!upgradeManager.CheckAttackConditionUpgrades(target)) // if it fails the upgrade can attack checks
        {
            return;
        }

        // Default Checks - these are handled differently to the replacement of targeting, becuse they are very simple and there are very few of them
        if (!upgradeManager.OverwriteBaseAllyCheck && BlockProperties.CurrentTeam == targetBlockProperties.CurrentTeam) // if the ally check has not been overwritten & if they are on the same team, do not attack
        {
            return;
        }


        int power = presetPower?? BlockProperties.PowerDict[direction]; // if a preset power has been input, then use this instead of the powerDict value

        if (!upgradeManager.OverwriteBaseNilPowerCheck && power == 0) // if the NilPower check has not been overwritten and if power is 0. This prevents sides with 0 power from hitting blocks
        {
            return;
        }

        upgradeManager.CheckAttackUpgrades(); //this check upgrades just before the actual attack

        spikeManager.SpikeAttackEffect(direction); // i think that this may need to be changed to take a direction as an input

        BaseOnHit(power, direction, target);


    }

    /// <summary>
    /// This is the function that actually hits the target, usually not called directly, unless wanting to hit without it being an attack
    /// </summary>
    /// <param name="power"></param>
    /// <param name="attackDir"></param>
    /// <param name="defender"></param>
    public void BaseOnHit(int power, GameTypes.DirectionEnum attackDir , BlockController defender) // this handles each individual hit, the attack function applies this to every target
    {

        bool didCapture = defender.BaseGetHit(attackDir.Invert(), power, this); // the side that gets hit is the opposite side than the attacker, e.g. an attack on the right side hits the defender's left

        upgradeManager.CheckOnHitUpgrades(didCapture, power, attackDir, defender);

    }



    /// <summary>
    /// This returns true if the targeted block was captured. 
    /// </summary>
    /// <param name="defendingDir">This is the direction of the defending block, usually the opposite to the attack direction</param>
    /// <param name="attackPower">The Power of the attack</param>
    /// <returns></returns>
    public bool BaseGetHit(GameTypes.DirectionEnum defendingDir, int attackPower, BlockController attacker) //I am not certain i want this to return a value, I will have to think about this a bit more
    {
        bool isCaptured = attackPower > BlockProperties.PowerDict[defendingDir]; //the defualt way of determining if is captured, ma be changed when I add upgrades that affect defense

        upgradeManager.CheckGetHitUpgrades(true, isCaptured, attackPower, defendingDir, attacker);
        //Debug.Log(BlockData.Sprite.name + " got hit at: " + Time.realtimeSinceStartupAsDouble);
        

        if (isCaptured)
        {
            GetCaptured(defendingDir);
        }

        upgradeManager.CheckGetHitUpgrades(false ,isCaptured, attackPower, defendingDir, attacker);
        return isCaptured;
    }


    /// <summary>
    /// Break the block, removing it from the board and putting it in the graveyard
    /// </summary>
    public void GetBroken()
    {
        // Trigger an on break event
        RemoveFromTile();
        BlockProperties.CurrentLocation = GameTypes.BlockLocation.Graveyard;
    }

    public void RemoveFromTile()
    {
        Tile tile = GridManager.Instance.Tiles[transform.position];
        tile.ClearTileContents();
    }


    #region Team Management

    /// <summary>
    /// This changes the blocks current team & flips it
    /// </summary>
    public void ChangeTeam(GameTypes.DirectionEnum defendingDir = GameTypes.DirectionEnum.Down) //this is currently called to make the enemy's blocks on the correct team, this may have to be changed as it is broadcasting events that may be needed elsewhere
    {

        BlockProperties.CurrentTeam = GameUtilities.ToggleTeam(BlockProperties.CurrentTeam);

        CoroutineRegistry.RunAndTrack(this, BlockFlip(defendingDir), true);

    }

    /// <summary>
    /// Captures this block, changing its team & flips it
    /// </summary>
    /// <param name="defendingDir">The direction the of the defending side</param>
    public void GetCaptured(GameTypes.DirectionEnum defendingDir) // Is this function necessary now? I may just be able to use the chage team funciton
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
    IEnumerator BlockFlip(GameTypes.DirectionEnum defDir)
    {

        Vector2 axis = -Vector2.Perpendicular(defDir.Invert().ToVector2());

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
        if (BlockProperties.CurrentTeam == GameTypes.Team.Player)
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
