using UnityEngine;

public class FragileUpgrade : BlockUpgrade, IGetHitUpgrade
{
    public override int UpgradeID => 7;

    public int getHitBehaviourPriority => -5; //Low priority so any other get hit effects happen first, then instead of getting caputred, it gets broken

    public bool isBeforeCapture => true;

    public void GetHitBehaviour(bool isCaptured, int attackPower, GameTypes.DirectionEnum defendDir, BlockController attacker)
    {
        if (isCaptured)
        {
            BlockController.GetBroken();
        }
    }
}
