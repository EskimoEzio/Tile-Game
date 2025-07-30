using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    
    //These are public so that they can be seen in the editor and read by other scripts, using properties to acomplish this is annoying if i want to serialise it or set default values
    public int gridWidth = 5;
    public int gridHeight = 5;

    [SerializeField] private Tile tilePrefab;
    public Dictionary<Vector2, Tile> Tiles = new Dictionary<Vector2, Tile>();


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GenerateGrid();

    }


    void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector2(x, y), Quaternion.identity, this.transform);
                spawnedTile.name = $"Tile ({x} , {y})";

                Tiles.Add(new Vector2(x, y), spawnedTile);

                if (x % 2 != y % 2)
                {
                    spawnedTile.GetComponent<Tile>().IsOffset = true;
                }
            }
        }

        //Set camera position based on grid location
        float xpos = gridWidth / 2 - 0.5f;  // this sets the x position in line with the middle of the grid with .5 offset so the camera is in the middle
        float ypos = gridHeight / 2 - 0.5f; // same as above
        float zpos = -10; // make sure that the camera is above everything else

        Camera.main.transform.position = new Vector3(xpos, ypos, zpos);
    }





}
