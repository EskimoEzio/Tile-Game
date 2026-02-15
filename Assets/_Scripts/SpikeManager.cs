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
    private SpriteRenderer blockRenderer;


    public Dictionary<Vector2, int> BlockPowerDict { get; private set; }

    [SerializeField] public float SpikeGap = 0.025f; // serialse field is not necessary here as it is public, it is just for clarity. also, ideal would be get;privateset, but this cannot be serialsed



    private void Awake()
    {
        blockObject = this.transform.parent.gameObject;
        blockController = blockObject.GetComponent<BlockController>();
        blockRenderer = blockObject.GetComponent<SpriteRenderer>();

        InitialiseArrays();

    }

    private void Start()
    {
        BlockPowerDict = blockController.PowerDict;
        



        SetRowPositions();

        if(blockController.CurrentTeam == GameTypes.Team.Player)
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


    public void SpikeAttackEffect()
    {

        CoroutineRegistry.RunAndTrack(this, SpikeMovement(), true);


    }
    
    IEnumerator SpikeMovement()
    {

        float halfDuration = 0.1f;
        float distance = 0.1f;
        float elapsedTime = 0f;

        // Caching the transforms can help with performance
        Transform upT = upSpikes.transform;
        Transform rightT = rightSpikes.transform;
        Transform downT = downSpikes.transform;
        Transform leftT = leftSpikes.transform;

        Vector2 upStart = upT.localPosition;
        Vector2 rightStart = rightT.localPosition;
        Vector2 downStart = downT.localPosition;
        Vector2 leftStart = leftT.localPosition;

        Vector2 upEnd = upStart + Vector2.up * distance;
        Vector2 rightEnd = rightStart + Vector2.right * distance;
        Vector2 downEnd = downStart + Vector2.down * distance;
        Vector2 leftEnd = leftStart + Vector2.left * distance;

        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;

            float lerpT = elapsedTime / halfDuration;

            upT.localPosition = Vector2.Lerp(upStart, upEnd, lerpT * lerpT); // doing lerpT * lerpT wil make it slow to start and then thrust quickly
            rightT.localPosition = Vector2.Lerp(rightStart, rightEnd, lerpT * lerpT);
            downT.localPosition = Vector2.Lerp(downStart, downEnd, lerpT * lerpT);
            leftT.localPosition = Vector2.Lerp(leftStart, leftEnd, lerpT * lerpT);

            yield return null;
        }
        //just in case the end of the animation is missed
        upT.localPosition = upEnd;
        rightT.localPosition = rightEnd;
        downT.localPosition = downEnd;
        leftT.localPosition = leftEnd;

        elapsedTime = 0;

        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;

            float lerpT = elapsedTime / halfDuration;

            upT.localPosition = Vector2.Lerp(upEnd, upStart, lerpT);
            rightT.localPosition = Vector2.Lerp(rightEnd, rightStart, lerpT);
            downT.localPosition = Vector2.Lerp(downEnd, downStart, lerpT);
            leftT.localPosition = Vector2.Lerp(leftEnd, leftStart, lerpT);

            yield return null;
        }

        // ensure that the sikes are put in their original positions
        upT.localPosition = upStart;
        rightT.localPosition = rightStart;
        downT.localPosition = downStart;
        leftT.localPosition = leftStart;

    }
    
}
