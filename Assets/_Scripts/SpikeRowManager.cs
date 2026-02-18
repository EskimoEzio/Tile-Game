using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpikeRowManager : MonoBehaviour
{
    private SpikeManager spikeManager;
    private Sprite spikeSprite;
    
    
    
    private GameObject[] spikes = new GameObject[5]; // 5 is the max number of spikes per row
    private SpriteRenderer[] spikeRenderers = new SpriteRenderer[5]; // 5 is the max number of spikes per row

    public GameTypes.DirectionEnum Direction;

    private float SpikeGap;
    //private float spikeWidth;


    private void Awake()
    {
        spikeManager = transform.parent.GetComponent<SpikeManager>();
        spikeSprite = transform.parent.GetComponent<SpikeManager>().SpikeSprite;

        InitialiseArrays(); //set up the arrays for the GameObjects and SpriteRenderers
    }


    private void Start()
    {
        



        SetBaseSpikeSprite(); // this is not strickly necessary atm, as i have already set the sprites, however this is good if i want to chage the spike sprite
        SpreadSpikes();
        DisableExcessSpikes();

    }

    void InitialiseArrays()
    {
        //Game Objects
        for (int i = 0; i < transform.childCount; i++)
        {
            spikes[i] = transform.GetChild(i).gameObject;
        }

        // Sprite Renderers
        for (int i = 0; i < spikes.Length; i++)
        {
            spikeRenderers[i] = spikes[i].GetComponent<SpriteRenderer>();

        }
    }

    public void SetBaseSpikeSprite()
    {
        for (int i = 0; i < spikes.Length; i++)
        {
            spikeRenderers[i].sprite = spikeSprite;
        }
    }

    void SpreadSpikes()
    {
        float spikeWidth = spikeSprite.bounds.size.x;
        float fullWidth = spikeWidth + spikeManager.SpikeGap;
        float xOffset = fullWidth / 4; // this will be modified based on which spike it is, this will allow it to alternate

        for(int i = 0; i < spikes.Length; i++)
        {
            float offsetDirection = -((i % 2) * 2 - 1); // this controls which side the current spike will be placed (L/R) relative to centre spike

            int offsetMult = (i+1)/2; // this controls the distance from the center - 0, 1, 1, 2, 2, 3...

            float xPos = xOffset + fullWidth * offsetMult * offsetDirection;
            spikes[i].transform.localPosition = new Vector2(xPos, 0);

        }
    }

    void DisableExcessSpikes() //this function deactivates any excess spikes
    {

        int power = spikeManager.BlockProperties.PowerDict[Direction]; //this finds the power by looking at the block properties

        for(int i = 0; i < spikes.Length; i++)
        {
            if(i>= power)
            {
                spikes[i].SetActive(false);
            }
        }

    }

    public void SetSpikeColour(Color colour)
    {
        for(int i = 0; i < spikes.Length; i++)
        {
            spikeRenderers[i].color = colour;
        }
    }

    
    public IEnumerator SpikeMovement(GameTypes.DirectionEnum direction)
    {

        float halfDuration = .2f;
        float distance = 0.1f;
        float elapsedTime = 0f;

        //Transform spikeRowTransform = upSpikes.transform; //by default use upspikes as this has to be assigned inorder to use it below
        Vector2 rowStartPos = transform.localPosition;
        Vector2 rowEndPos = rowStartPos + direction.ToVector2() * distance;


        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;

            float lerpT = elapsedTime / halfDuration;

            transform.localPosition = Vector2.Lerp(rowStartPos, rowEndPos, lerpT * lerpT); // doing lerpT * lerpT wil make it slow to start and then thrust quickly


            yield return null;
        }
        //just in case the end of the animation is missed
        transform.localPosition = rowEndPos;


        elapsedTime = 0;

        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;

            float lerpT = elapsedTime / halfDuration;

            transform.localPosition = Vector2.Lerp(rowEndPos, rowStartPos, lerpT);

            yield return null;
        }

        // ensure that the sikes are put in their original positions
        transform.localPosition = rowStartPos;
    }


}
