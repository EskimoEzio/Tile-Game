using UnityEngine;
using System.Collections.Generic;

public interface ISearchReplacementUpgrade
{
    public List<TargetInfo> Search(AttackInfo attackInfo, GameTypes.DirectionEnum direction, SearchParameters searchParameters);
}
