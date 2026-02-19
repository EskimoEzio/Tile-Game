using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameTypes
{
    public enum Team { Player, Enemy }
    public enum Turn { Player, Enemy }
    
    public enum DirectionEnum { Up, Right, Down, Left }

    public static readonly DirectionEnum[] AllDirections = { DirectionEnum.Up, DirectionEnum.Right, DirectionEnum.Down, DirectionEnum.Left };

    public enum BlockLocation { Hand, Board } //what other locations might I need, library, discard/graveyard, none?
}
