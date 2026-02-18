using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTypes
{


    public enum Team { Player, Enemy }

    public enum Turn { Player, Enemy }
    
    public enum DirectionEnum { Up, Right, Down, Left }

    public static readonly DirectionEnum[] AllDirections = { DirectionEnum.Up, DirectionEnum.Right, DirectionEnum.Down, DirectionEnum.Left };

    //public static readonly Vector2[] Directions = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

}
