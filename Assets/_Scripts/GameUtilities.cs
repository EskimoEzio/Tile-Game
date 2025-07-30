using UnityEngine;

public static class GameUtilities
{
    
    /// <summary>
    /// Compares a given turn to a team to see if they match
    /// </summary>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    public static bool CheckTurnMatchTeam(GameTypes.Turn turn, GameTypes.Team team)
    {
        return (turn == GameTypes.Turn.Player && team == GameTypes.Team.Player) || (turn == GameTypes.Turn.Enemy && team == GameTypes.Team.Enemy);
    }


    public static GameTypes.Team ToggleTeam(GameTypes.Team curTeam) //this methd is purely for changing the enum between enemy & player
    {
        if (curTeam == GameTypes.Team.Player)
        {
            return GameTypes.Team.Enemy;
        }
        else if (curTeam == GameTypes.Team.Enemy)
        {
            return GameTypes.Team.Player;
        }
        else
        {
            Debug.LogError("current team is not player or enemy");
            return curTeam;
        }
    }

}
