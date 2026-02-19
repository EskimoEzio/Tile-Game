using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikeManager : MonoBehaviour
{

    [SerializeField] private GameObject upSpikes;
    [SerializeField] private GameObject rightSpikes;
    [SerializeField] private GameObject downSpikes;
    [SerializeField] private GameObject leftSpikes;

    private GameObject[] spikeRows = new GameObject[4];
    private SpikeRowManager[] spikeRowManagers = new SpikeRowManager[4];

    [SerializeField] public Sprite SpikeSprite; // this is public so that it can be serialised and it needs to be accessed by spikeRowManager. Ther [serialisefield] is just for clarity that it is assigned in inspector


    private GameObject blockObject; // this is the parent game object
    private BlockController blockController;
    public BlockProperties BlockProperties;
    private SpriteRenderer blockRenderer;


    [SerializeField] public float SpikeGap = 0.025f; // serialse field is not necessary here as it is public, it is just for clarity. also, ideal would be get;privateset, but this cannot be serialsed

    private void Awake()
    {
        blockObject = this.transform.parent.gameObject;
        blockController = blockObject.GetComponent<BlockController>();
        BlockProperties = blockController.BlockProperties;
        blockRenderer = blockObject.GetComponent<SpriteRenderer>();

        InitialiseArrays();

    }

    private void Start()
    {


        SetRowPositions();

        if(BlockProperties.CurrentTeam == GameTypes.Team.Player)
        {
            SetSpikeRowColour(blockController.PlayerColour);
        }
        else
        {
            SetSpikeRowColour(blockController.EnemyColour);
        }
    }

    void SetRowPositions()
    {

        float yOffset = blockRenderer.bounds.extents.y + SpikeSprite.bounds.extents.y;

        // This only works if the block is a square 
        upSpikes.transform.localPosition = new Vector2(0, yOffset);
        rightSpikes.transform.localPosition = new Vector2(yOffset, 0);
        downSpikes.transform.localPosition = new Vector2(0, -yOffset);
        leftSpikes.transform.localPosition = new Vector2(-yOffset, 0);
    }


    void InitialiseArrays()
    {
        spikeRows[0] = upSpikes;
        spikeRows[1] = rightSpikes;
        spikeRows[2] = downSpikes;
        spikeRows[3] = leftSpikes;


        for (int i = 0; i < spikeRowManagers.Length; i++)
        {
            spikeRowManagers[i] = spikeRows[i].GetComponent<SpikeRowManager>();
            spikeRowManagers[i].Direction = (GameTypes.DirectionEnum)i; //this should automatically set the dirction to the matching enum, becuase I always go clockwise sstarting from up
        }
    }

    public void SetSpikeRowColour(Color spikeRowColour)
    {
        //print(spikeRowColour);
        for (int i = 0; i < spikeRowManagers.Length; i++)
        {
            spikeRowManagers[i].SetSpikeColour(spikeRowColour);
        }
    }


    public void UpdateVisibleSpikes()
    {
        foreach(SpikeRowManager spikeRowManager in spikeRowManagers)
        {
            spikeRowManager.UpdateVisibleSpikes();
        }
    }



    /// <summary>
    /// This is the fucntion that moves a single row of spikes
    /// </summary>
    /// <param name="rowToMove">This is which row should move, represented my a vector 2 with magnitude 1</param>
    public void SpikeAttackEffect(GameTypes.DirectionEnum rowToMove)
    {
        int index = (int)rowToMove; //doing it this way allows me to avoid having a switch statement for each case, as the index for the enum and the rows match by design
        CoroutineRegistry.RunAndTrack(this, spikeRowManagers[index].SpikeMovement(rowToMove), true);

    }
}
