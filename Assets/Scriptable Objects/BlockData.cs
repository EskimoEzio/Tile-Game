using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BlockData", menuName = "Scriptable Objects/BlockData")]
public class BlockData : ScriptableObject
{

    public string BlockName;
    public Sprite Sprite; // this may need to be an image, I will look into it

    public int[] PowerValues = new int[4]; // these values are N,E,S,W (so starting north and working clockwise)

    public int attackRange = 1; //This only applies when the asset is first created, changing it now does not change any already created SOs
    
    [SerializeReference] //serialise reference is used instead of field as the upgrade types can vary (block upgrade is abstract)
    public List<BlockUpgrade> DefaultUpgrades = new(); //This is the list of the upgrades held by a blick by default.

}
