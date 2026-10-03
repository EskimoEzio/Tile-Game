using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Mortar2 : BlockUpgrade, ISearchReplacementUpgrade
{
    public override int UpgradeID { get; } = 11;

    [SerializeField] private int exampleValue = 0;

    public List<TargetInfo> Search(AttackInfo attackInfo, GameTypes.DirectionEnum direction, int range)
    {
        List<TargetInfo> targets = new List<TargetInfo>();

        Vector2 targetLocation = (Vector2)attackInfo.Attacker.gameObject.transform.position + direction.ToVector2() * range; // target location is exactly attack range away

        if (!GridManager.Instance.Tiles.ContainsKey(targetLocation)) //if tile doesnt exist, then there is no target
        {
            return targets;
        }
        else
        {
            targets.Add(new TargetInfo(GridManager.Instance.Tiles[targetLocation]));

            return targets;
        }

    }
}
