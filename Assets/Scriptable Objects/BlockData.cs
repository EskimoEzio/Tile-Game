using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "Scriptable Objects/BlockData")]
public class BlockData : ScriptableObject
{

    public string BlockName;
    public Sprite Sprite; // this may need to be an image, I will look into it

    public int[] PowerValues = new int[4]; // these values are N,E,S,W (so starting north and working clockwise



}
