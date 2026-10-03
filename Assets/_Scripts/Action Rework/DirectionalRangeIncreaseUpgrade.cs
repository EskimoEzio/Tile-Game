using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class DirectionalRangeIncreaseUpgrade : BlockUpgrade, ISearchModifierUpgrade
{
    public override int UpgradeID { get; } = 12;

    [SerializeField] private int rangeIncrease = 1;

    [SerializeField] private List<GameTypes.DirectionEnum> buffDirections;

    public void ModifySearch(SearchParameters searchParameters, GameTypes.DirectionEnum direction)
    {
        if (buffDirections.Contains(direction))
        {
            searchParameters.Range += rangeIncrease;
            Debug.Log("buff");
        }

    }

}
