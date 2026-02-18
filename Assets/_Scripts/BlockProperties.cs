using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlockController))]
public class BlockProperties : MonoBehaviour
{

    private UpgradeManager upgradeManager;

    public BlockData BlockData;
    //[SerializeField] private BlockData defaultData;


    [Header("Base Stats")]
    private int baseAttackRange;
    public Dictionary<GameTypes.DirectionEnum, int> BasePowerDict { get; private set; }



    [Header("Final Stats")]
    public int AttackRange { get; private set; }
    public Dictionary<GameTypes.DirectionEnum, int> PowerDict { get; private set; }




    public bool IsPlaced; //Done
    public GameTypes.Team CurrentTeam;


    private void Awake()
    {
        //Create the instances of the power dictionaries
        BasePowerDict = new Dictionary<GameTypes.DirectionEnum, int>();
        PowerDict = new Dictionary<GameTypes.DirectionEnum, int>();

        //InitialiseBaseStats();
    }

    private void Start()
    {
        InitialiseStats();
        
    }

    public void InitialiseStats()
    {
        // Base Attack Range
        baseAttackRange = BlockData.attackRange;

        // Base Power Dictionary
        Debug.Assert(BlockData.PowerValues.Length == 4, "The length of the power list is not 4" + gameObject.name); //raises an error if the power list length is not 4
        for(int i = 0; i<4; i++) // 4 is used becuase there are 4 directions
        {
            BasePowerDict.Add((GameTypes.DirectionEnum)i, BlockData.PowerValues[i]); //Create the base power dictionary
            PowerDict.Add((GameTypes.DirectionEnum)i, BlockData.PowerValues[i]); // create the empty dictionary for the final power values, the values will be filled when upgrades are checked
        }


        UpdateStats();
    }

    /// <summary>
    /// This checks all upgrades to see what the current stats should be
    /// </summary>
    public void UpdateStats()
    {

        // For now it just sets the final stats equal to the base stats

        AttackRange = baseAttackRange;

        foreach (GameTypes.DirectionEnum direction in BasePowerDict.Keys)
        {
            PowerDict[direction] = BasePowerDict[direction]; // Set the values of powerdict equal to the BasePowerdict
        }

    }

}
