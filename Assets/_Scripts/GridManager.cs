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

        Camera.main.transform.position = new Vector3(gridWidth / 2 - 0.5f, gridHeight / 2 - 0.5f, -10);
    }





}
