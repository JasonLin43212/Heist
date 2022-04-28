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
    public Collider2D playerCollider;

    // References
    public GameObject rangeDisplayObject;
    public Animator animator;

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
        itemContactFilter.useTriggers = true;
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw(x_axis);
        movement.y = Input.GetAxisRaw(y_axis);

        // 0: idle, 1: up, 2: right, 3: down, 4: left
        int movementId = 0;
        if (movement.y > 0) { movementId = 1; }
        else if (movement.y < 0) { movementId = 3; }
        else if (movement.x > 0) { movementId = 2; }
        else if (movement.x < 0) { movementId = 4; }
        animator.SetInteger("WalkDirection", movementId);

        HandleItems();
    }

    void FixedUpdate()
    {
        Rigidbody2D myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.MovePosition(myRigidbody.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Door" && collision.gameObject.GetComponent<ClickDoorBehavior>().shouldDoorBeClosed == false)
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<BoxCollider2D>(), playerCollider);
        }
    }

    private void HandleItems()
    {
        ItemManager itemManager = GameState.Instance.ItemManager;

        if (Input.GetKeyDown(pickDropKey))
        {
            // Look to see if we're touching any items
            Collider2D[] colliderBuffer = new Collider2D[GameConfig.ITEM_COLLIDER_BUFFER_SIZE];
            int numOverlappingColliders = GetComponent<Collider2D>().OverlapCollider(itemContactFilter, colliderBuffer);

            // If currently holding an item, drop it
            if (itemManager.PlayerIsHoldingItem(player))
            {
                ItemBehaviour heldItem = itemManager.GetHeldItemBehaviour(player);
                heldItem.Drop();
            }

            // If we are overlapping any other item, pick it up
            if (numOverlappingColliders > 0)
            {
                // Try to pick up the item
                GameObject overlappingItem = colliderBuffer[0].transform.parent.gameObject;
                overlappingItem.GetComponent<ItemBehaviour>().Pickup(player);  // is true if pick up was successful
            }
        }

        if (itemManager.PlayerIsHoldingItem(player))
        {
            // Check if item is being used
            ItemBehaviour heldItem = itemManager.GetHeldItemBehaviour(player);
            heldItem.CheckForUse(player);
        }
    }

    public void SetRange(float worldRadius)
    {
        rangeDisplayObject.SetActive(true);
        rangeDisplayObject.transform.localScale = new Vector3(2.5f * worldRadius, 2.5f * worldRadius, 1f);
    }

    public void HideRange()
    {
        rangeDisplayObject.SetActive(false);
    }

    public Collider2D GetRangeCollider()
    {
        return rangeDisplayObject.GetComponent<Collider2D>();
    }
}
