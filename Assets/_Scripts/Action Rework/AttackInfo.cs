using UnityEngine;
using System.Collections.Generic;

public class AttackInfo
{
    public BlockController Attacker;
    public List<SideAttackInfo> SideAttacks;
    public List<HitResolutionInfo> PendingHitResults; //this contains the hitInfo and the hitResultInfo

    public AttackInfo(BlockController attacker) //at a later point i may wish to change this to accept a sideAttackInfo object so that i can do attacks in single directiosn - but that is unnecessaru at this point
    {
        Attacker = attacker;
        PendingHitResults = new List<HitResolutionInfo>();
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

public class SearchParameters
{
    public int Range;
    public bool StopAtFirstOccupied = true; // this is the default behaviour
    //public bool AlliesBlock = true; // i am not certain how i want to incorporate this, so i will ignore it for now


    public SearchParameters(int range)
    {
        Range = range;
    }
}

public class HitInfo
{
    public BlockController Attacker;
    public BlockController Defender;
    public GameTypes.DirectionEnum AttackDirection;
    public int CurrentPower;


    //create constructor

    public HitInfo(BlockController attacker, BlockController defender, GameTypes.DirectionEnum attackDirection, int currentPower)
    {
        Attacker = attacker;
        Defender = defender;
        AttackDirection = attackDirection;
        CurrentPower = currentPower;
    }

}

/// <summary>
/// This holds the HitInfo aswell as the effective powers for teh attacker aand defender
/// </summary>
public class ReceiveHitContext
{
    public HitInfo HitInfo;
    public int AttackerPower;
    public int DefenderPower;

    public ReceiveHitContext(HitInfo hitInfo, int attackerPower, int defenderPower)
    {
        HitInfo = hitInfo;
        AttackerPower = attackerPower;
        DefenderPower = defenderPower;
    }
}


public enum HitOutcome
{
    NoChange,
    Captured,
    Broken
}

/// <summary>
/// Stores the basic information regarding the result of a hit
/// </summary>
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

/// <summary>
/// This contains the HitInfo aswell as the HitResultInfo
/// </summary>
public class HitResolutionInfo
{
    public HitInfo HitInfo;
    public HitResultInfo HitResultInfo;

    public HitResolutionInfo(HitInfo hitInfo, HitResultInfo hitResultInfo)
    {
        HitInfo = hitInfo;
        HitResultInfo = hitResultInfo;
    }
}
