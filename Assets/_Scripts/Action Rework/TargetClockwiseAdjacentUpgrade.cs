using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TargetClockwiseAdjacentUpgrade : BlockUpgrade, ITargetModifierUpgrade
{
    public override int UpgradeID { get; } = 14;

    public void ModifyTargets(AttackInfo attackInfo, GameTypes.DirectionEnum direction, List<TargetInfo> targets)
    {
        List<TargetInfo> additionalTargets = new();
        
        foreach(TargetInfo targetInfo in targets)
        {
            Vector2 targetoffset = direction.Rotate().ToVector2();

            Vector2 targetLocation = (Vector2)targetInfo.TargetTile.transform.position + targetoffset;

            if (!GridManager.Instance.Tiles.ContainsKey(targetLocation)) // if the grid does NOT contain the key
            {
                continue;
            }

            additionalTargets.Add(new TargetInfo(GridManager.Instance.Tiles[targetLocation]));
        }

        targets.AddRange(additionalTargets);
    }
}
