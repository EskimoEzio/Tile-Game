using UnityEngine;
using System.Collections.Generic;

public class AttackInfo
{
    public BlockController Attacker;
    public List<SideAttackInfo> SideAttacks;


    public AttackInfo(BlockController attacker) //at a later point i may wish to change this to accept a sideAttackInfo object so that i can do attacks in single directiosn - but that is unnecessaru at this point
    {
        Attacker = attacker;
    }
}

public class SideAttackInfo
{
    public GameTypes.DirectionEnum Direction;
    public List<TargetInfo> Targets;

    public SideAttackInfo(GameTypes.DirectionEnum direction, List<TargetInfo> targets)
    {
        Direction = direction;
        Targets = targets;
    }

}

public class TargetInfo
{
    public Tile TargetTile;
    // may add additional info here later (e.g. directional and perpendicular distance)

    public TargetInfo(Tile targetTile)
    {
        TargetTile = targetTile;
    }

}

public class HitInfo
{
    public BlockController Attacker;
    public BlockController Target;
    public GameTypes.DirectionEnum AttackDirection;
    public int CurrentPower;


    //create constructor

    public HitInfo(BlockController attacker, BlockController target, GameTypes.DirectionEnum attackDirection, int currentPower)
    {
        Attacker = attacker;
        Target = target;
        AttackDirection = attackDirection;
        CurrentPower = currentPower;
    }

}

public enum HitOutcome
{
    NoChange,
    Captured,
    Broken
}

public class HitResultInfo
{

    public bool HitOccurred;
    public HitOutcome FinalOutcome;

    public HitResultInfo(bool hitOccurred, HitOutcome finalOutcome)
    {
        HitOccurred = hitOccurred;
        FinalOutcome = finalOutcome;
    }

}
