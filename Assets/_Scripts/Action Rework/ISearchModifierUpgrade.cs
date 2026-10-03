using UnityEngine;

public interface ISearchModifierUpgrade
{
    void ModifySearch(SearchParameters searchParameters, GameTypes.DirectionEnum direction);
}
