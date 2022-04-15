using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Player player;
    public float moveSpeed = 10f;

    private Vector2 movement;
    public string x_axis;
    public string y_axis;

    // Controls
    private KeyCode pickDropKey;

    // Misc
    private ContactFilter2D itemContactFilter;

    // Start is called before the first frame update
    void Start()
    {
        if (player == Player.Player1)
        {
            x_axis = "Horizontal";
            y_axis = "Vertical";
        }
        else
        {
            x_axis = "Horizontal2";
            y_axis = "Vertical2";
        }

        pickDropKey = GameState.Instance.GetPickDropKey(player);

        itemContactFilter = new ContactFilter2D();
        itemContactFilter.SetLayerMask(LayerMask.GetMask("Item"));
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw(x_axis);
        movement.y = Input.GetAxisRaw(y_axis);

        HandleItems();
    }

    void FixedUpdate()
    {
        Rigidbody2D myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.MovePosition(myRigidbody.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleItems()
    {
        ItemManager itemManager = GameState.Instance.ItemManager;

        if (Input.GetKeyDown(pickDropKey))
        {
            if (itemManager.PlayerIsHoldingItem(player))
            {
                // Drop the current item
                Item heldItem = itemManager.GetHeldItem(player);
                heldItem.Drop();
            }
            else
            {
                // Look to see if we're touching any items
                Collider2D[] colliderBuffer = new Collider2D[GameConfig.ITEM_COLLIDER_BUFFER_SIZE];
                int numOverlappingColliders = GetComponent<Collider2D>().OverlapCollider(itemContactFilter, colliderBuffer);
                if (numOverlappingColliders > 0)
                {
                    // Try to pick up the item
                    GameObject overlappingItem = colliderBuffer[0].transform.parent.gameObject;
                    overlappingItem.GetComponent<Item>().Pickup(player);  // is true if pick up was successful
                }
            }
        }

        if (itemManager.PlayerIsHoldingItem(player))
        {
            // Check if item is being used
            Item heldItem = itemManager.GetHeldItem(player);
            heldItem.CheckForUse(player);
        }
    }
}
