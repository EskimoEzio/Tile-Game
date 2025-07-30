using UnityEngine;

public static class GameUtilities
{
    
    /// <summary>
    /// Compares a given turn to a team to see if they match
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    public static bool CheckTurnMatchTeam(TurnManager.Turn turn, BlockController.Team team)
    {
        return (turn == TurnManager.Turn.Player && team == BlockController.Team.Player) || (turn == TurnManager.Turn.Enemy && team == BlockController.Team.Enemy);
    }


}
