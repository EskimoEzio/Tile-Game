using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer tileRenderer;
    private Camera mainCam;
    private Color baseColor = new Color(0, 0.5f, 0.5f);
    private Color altColor = new Color(0, 0.5f, 0f);
    private float highlightMult = 2f;
    
    private Collider2D col;
    public GameObject TileContents { get; private set; } // This holds the block that is in the tile, it will be null if tile is empty

    public bool IsOffset; // this is set when the grid generation runs, it is not changed again. I tis easier to have it be public

    



    private void Awake()
    {
        tileRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        mainCam = Camera.main;
    }


    private void Start()
    {
        SetInitialColor();

    }


    private void OnEnable()
    {
        InputHandler.Instance.OnMouseMove += HandleMouseMove;
    }

    private void OnDisable()
    {
        InputHandler.Instance.OnMouseMove -= HandleMouseMove;
    }

    public void SetInitialColor()
    {
        
        // THis is a simpler way of writing the following if statement
        tileRenderer.color = IsOffset ? altColor : baseColor;

        /*
        if (IsOffset)
        {
            tileRenderer.color = altColor;
        }
        else
        {
            tileRenderer.color = baseColor;
        }*/
    }

    private void HandleMouseMove(Vector2 screenPosition)
    {
        // This has an issue where if the mouse is perfectly on a gridline (or inbetween colliders if using size of 0.99)
        // it causes the mouse to either hit (when using scale of 1) or miss (when using smaller scale) both colliders
        // I will fix this later, but it is not important now

        if (TurnManager.Instance.CurrentTurn != GameTypes.Turn.Player) // I will only highlight tiles if it is the players turn. Later this will be expanded to only if the block can be placed on the tile
        {
            return;
        }


        Vector2 worldPos = mainCam.ScreenToWorldPoint(screenPosition);
        if (col.OverlapPoint(worldPos)&& TileContents == null) //only highlight empty tiles
        {
            HighlightTile();
        }
        else
        {
            SetInitialColor();
        }
    }


    private void HighlightTile()
    {
        
        tileRenderer.color *= highlightMult;
    }


    public void AddToTile(GameObject block)
    {
        // I have made this into a method as there will be extra stuff to add to this later
        TileContents = block;
        block.transform.position = new Vector2(transform.position.x, transform.position.y);

        // I input the position as this is matches the Vector2 key in the tile dictionary
        block.GetComponent<BlockController>().PlaceBlock(transform.position);

    }


}
