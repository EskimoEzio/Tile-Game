using System;
using UnityEngine;

[Serializable]
public class SuperFragileUpgrade : BlockUpgrade, IGetHitUpgrade
{
    public override int UpgradeID { get; } = 8;

    public int getHitBehaviourPriority => -5; //Low priority so any other get hit effects happen first, then it gets broken regardless of power

    public bool isBeforeCapture => true;

    public void GetHitBehaviour(bool isCaptured, int attackPower, GameTypes.DirectionEnum defendDir, BlockController attacker)
    {
        BlockController.GetBroken();
    }
}
