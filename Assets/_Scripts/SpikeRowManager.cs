using UnityEngine;

public class SpikeRowManager : MonoBehaviour
{
    private SpikeManager spikeManager;
    private Sprite spikeSprite;
    
    
    
    private GameObject[] spikes = new GameObject[5]; // 5 is the max number of spikes per row
    private SpriteRenderer[] spikeRenderers = new SpriteRenderer[5]; // 5 is the max number of spikes per row

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
        
        int power = spikeManager.BlockPowerDict[(transform.position - transform.parent.position).normalized]; //this works out the relative position and uses that as the key for the dictionary

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


}
