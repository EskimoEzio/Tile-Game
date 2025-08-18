using UnityEngine;

public interface ITargetReplacementUpgrade : ITargetUpgrade
{
    // this is for any upgrades that will completely replace the targeting behaviour

    
    void Target(BlockController blockController);

}
