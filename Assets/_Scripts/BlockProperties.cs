using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlockController))]
public class BlockProperties : MonoBehaviour
{
    private BlockController blockController;
    public BlockData BlockData;

    [Header("Base Stats")]
    private int baseAttackRange;
    public Dictionary<GameTypes.DirectionEnum, int> BasePowerDict { get; private set; }


    [Header("Bonus Stats")]
    private int bonusAttackRange = 0;
    public Dictionary<GameTypes.DirectionEnum, int> BonusPowerDict { get; private set; }


    [Header("Final Stats")]
    public int AttackRange { get; private set; }
    public Dictionary<GameTypes.DirectionEnum, int> PowerDict { get; private set; }



    private int minimumRange = 1;
    private int minimumPower = 0;
    private int maximumPower = 5;

    public bool IsPlaced; 
    public GameTypes.Team CurrentTeam;


    private void Awake()
    {
        blockController = GetComponent<BlockController>();

        //Create the instances of the power dictionaries
        BasePowerDict = new Dictionary<GameTypes.DirectionEnum, int>();
        BonusPowerDict = new Dictionary<GameTypes.DirectionEnum, int>();
        PowerDict = new Dictionary<GameTypes.DirectionEnum, int>();
    }

    public void InitialiseStats()
    {
        baseAttackRange = BlockData.attackRange;

        Debug.Assert(BlockData.PowerValues.Length == 4, "The length of the power list is not 4" + gameObject.name); //raises an error if the power list length is not 4

        foreach (GameTypes.DirectionEnum direction in GameTypes.AllDirections)
        {
            BasePowerDict.Add(direction, BlockData.PowerValues[(int)direction]); //Create the base power dictionary
            BonusPowerDict.Add(direction, 0); //Bonus values start at 0, adjusted in the other functions
            PowerDict.Add(direction, BlockData.PowerValues[(int)direction]); // create the empty dictionary for the final power values, the values will be filled when upgrades are checked
        }
        
       UpdateStats();

    }

    /// <summary>
    /// This checks all upgrades to see what the current stats should be
    /// </summary>
    public void UpdateStats()
    {

        AttackRange = Mathf.Max(minimumRange, baseAttackRange + bonusAttackRange);

        foreach (GameTypes.DirectionEnum direction in BasePowerDict.Keys)
        {
            PowerDict[direction] = Mathf.Clamp(BasePowerDict[direction] + BonusPowerDict[direction], minimumPower, maximumPower); // Power is made up of base + bonus, but is currently limited to no less than 0 or more than 5
        }

        blockController.spikeManager.UpdateVisibleSpikes();

        int counter = 0;

        foreach(int pow in PowerDict.Values)
        {
            counter += pow;
        }

    }

    /// <summary>
    /// Modify bonus range by modifier amount
    /// </summary>
    /// <param name="modifier">The amount that the range should increase or decrease</param>
    public void ModifyRange(int modifier)
    {
        bonusAttackRange += modifier;
        UpdateStats(); // This seems like an inefficient way of doing this, maube i will just remove the updateStats function
    }

    /// <summary>
    /// Modify bonus power for all given directions. Write directions seperated by ","
    /// </summary>
    /// <param name="modifier">The amount the power will be modified by</param>
    /// <param name="directions">All directions to be modified, write them seperated by ","</param>
    public void ModifyPower(int modifier, params GameTypes.DirectionEnum[] directions)
    {
        foreach(GameTypes.DirectionEnum direction in directions)
        {
            BonusPowerDict[direction] += modifier;
        }
        
        
        UpdateStats(); // This seems like an inefficient way of doing this, maube i will just remove the updateStats function
    }

    /// <summary>
    /// Modify bonus power for any nubmer of random directions. Only allows the modifier to apply once per direction
    /// </summary>
    /// <param name="modifier">The amount the power will be modified by</param>
    /// <param name="numberOfSides">number of random sides to apply modifier to</param>
    public void ModifyPower(int modifier, int numberOfSides)
    {
        // print("momentum: " + BlockData.BlockName);
        // print("number of sides to modify: " + numberOfSides);

        Debug.Assert(numberOfSides < 4, "Number of sides to modify is greater than 4");
        
        List<GameTypes.DirectionEnum> availableDirections = new List<GameTypes.DirectionEnum>(GameTypes.AllDirections);

        
        // This prevents it from adding power to any sides that are already at the power cap
        foreach(GameTypes.DirectionEnum direction in GameTypes.AllDirections)
        {
            if(PowerDict[direction] >= maximumPower)
            {
                availableDirections.Remove(direction);
            }
        }
        if(availableDirections.Count == 0)
        {
            UpdateStats();
            return;
        }


        for( int i = 0; i < numberOfSides; i++)
        {
            int index = UnityEngine.Random.Range(0, availableDirections.Count);

            BonusPowerDict[availableDirections[index]] += modifier;

            availableDirections.RemoveAt(index);
        }

        //BonusPowerDict[direction] += modifier;
        UpdateStats(); // This seems like an inefficient way of doing this, maube i will just remove the updateStats function
    }

}
