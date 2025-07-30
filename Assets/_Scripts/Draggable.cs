using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))] // require a collider2d as this is important for knowing if the mouse is over the block
public class DraggableTile : MonoBehaviour
{

    private Camera mainCam;
    private List<HandManager> allHands;
    private HandManager ownerHand;

    private BlockController blockController;

    private LayerMask placeableLayers; // This layermask holds all of the places where a block could be dragged to


    private bool isDragging;
    private bool canBeDragged = true;

    private float zOffset = -1f; // this is the lift when moving a block


    // when the block is enabled, subscribe the appropriate methods to the "OnMouse" events, this is so that they are triggered
    private void OnEnable()
    {
        InputHandler.Instance.OnMouseDown += HandleMouseDown;
        InputHandler.Instance.OnMouseUp += HandleMouseUp;
        InputHandler.Instance.OnMouseMove += HandleMouseMove;
    }


    // when the block is disabled, unsubscribe from the events, this is important as it helps to avoid duplicate calls, memory leaks and null errors
    private void OnDisable()
    {
        InputHandler.Instance.OnMouseDown -= HandleMouseDown;
        InputHandler.Instance.OnMouseUp -= HandleMouseUp;
        InputHandler.Instance.OnMouseMove -= HandleMouseMove;
    }


    private void Awake()
    {
        mainCam = Camera.main;
        //hand = FindAnyObjectByType<HandManager>();
        //placeableLayers = LayerMask.GetMask("Tile", "Hand");
        placeableLayers = LayerMask.GetMask("Tile");

        allHands = new List<HandManager>(FindObjectsByType<HandManager>(FindObjectsSortMode.None));
        blockController = GetComponent<BlockController>();

    }


    private void Start()
    {
        
        // Assign the correct hand to ownerHand
        foreach(HandManager hand in allHands)
        {
            if(hand.handTeam == blockController.CurrentTeam)
            {
                ownerHand = hand;
            }
        }
    }

    void HandleMouseDown(Vector2 screenPos)
    {
        // if cannot be dragged (currently this is only when on a tile) then return (do not pick upt the tile)
        if (!canBeDragged)
        {
            return;
        }

        if (blockController.IsPlaced)
        {
            canBeDragged = false;
            return;
        }
            
        if (!GameUtilities.CheckTurnMatchTeam(TurnManager.Instance.ActivePlayer, blockController.CurrentTeam)) // this compares the Active player to the target block to see if they should be able to click it
        {
            return;
        }

        
        Vector3 worldPos = mainCam.ScreenToWorldPoint(screenPos);
        if (GetComponent<Collider2D>().OverlapPoint(worldPos))
        {
            isDragging = true;
            ownerHand.RemoveFromHand(gameObject);

            // if you are dragging it, change the z by a small amount so that it shows above other blocks
            worldPos.z = zOffset;
            transform.position = worldPos;
        }
    }

    
    void HandleMouseUp(Vector2 screenPos)
    {
       
        //first checking if i am draggin something, by doing it this way, i dont have to indent the rest of the important text
        if (!isDragging)
        {
            return;
        }
        
        isDragging = false;

        // now that it has been dropped, set the z back to the standard
        transform.position = transform.position + Vector3.forward;

        // first get the world pos of the mouse
        Vector2 worldPos = mainCam.ScreenToWorldPoint(screenPos);


        if(!GameUtilities.CheckTurnMatchTeam(TurnManager.Instance.ActivePlayer, blockController.CurrentTeam)) // if the block does not match the turn put this into ownerHand
        {
            ownerHand.AddToHand(gameObject, screenPos);
            return;
        }

        

        //Check if it can be placed on the tile below, if not return to hand. Although i also want click on block then click on target tile. But I willl worry about this later
        Collider2D targetCollider = Physics2D.OverlapPoint(worldPos, placeableLayers);
        
        if(targetCollider == null)
        {
            //if the block is not held over anything in the layermask, then return it to your hand
            ownerHand.AddToHand(gameObject, screenPos);
            return;
        }
      

        GameObject targetObject = targetCollider.gameObject;
 
        //check if hovering a tile
        if(targetObject.TryGetComponent<Tile>(out Tile tile))
        {

            //Check if the block can be placed on the tile, ATM this will just be checking if it is empty
            if(tile.TileContents == null)
            {
                tile.AddToTile(gameObject);
                canBeDragged = false;

                TurnManager.Instance.EndTurn();
            }
            else
            {
                // if the tile is not empty, add the tile to your hand
                ownerHand.AddToHand(gameObject, screenPos);
            }
        }
        else if(targetObject.TryGetComponent<HandManager>(out HandManager hand)) //Seemingly unecessary
        {
           
            //hand.AddToHand(gameObject);
        }

    }

    void HandleMouseMove(Vector2 screenPos)
    {
        if (!isDragging) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = zOffset;
        transform.position = worldPos;
    }


}
