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

    /// <summary>
    /// Toggls the given enum between player and enemy
    /// </summary>
    /// <param name="curTeam"></param>
    /// <returns></returns>
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

    public static GameTypes.Turn ToggleTurn(GameTypes.Turn curTurn) //this metohd is purely for changing the enum between enemy & player
    {
        if (curTurn == GameTypes.Turn.Player)
        {
            return GameTypes.Turn.Enemy;
        }
        else if (curTurn == GameTypes.Turn.Enemy)
        {
            return GameTypes.Turn.Player;
        }
        else
        {
            Debug.LogError("current turn is not player or enemy");
            return curTurn;
        }
    }


    /// <summary>
    /// This converts a direction enum to a normalised Vector2
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Vector2 ToVector2(this GameTypes.DirectionEnum direction)
    {
        switch (direction)
        {
            case GameTypes.DirectionEnum.Up: return Vector2.up;
            case GameTypes.DirectionEnum.Right: return Vector2.right;
            case GameTypes.DirectionEnum.Down: return Vector2.down;
            case GameTypes.DirectionEnum.Left: return Vector2.left;
            default: return Vector2.zero;
        }
    }

    public static GameTypes.DirectionEnum Invert(this GameTypes.DirectionEnum direction)
    {
        return (GameTypes.DirectionEnum)(((int)direction + 2) % 4); //doing it this way aboids a dwitch statement. basically adds 2 to the enum index then divides by 4 and uses the remainder as the output index 
    }


}
