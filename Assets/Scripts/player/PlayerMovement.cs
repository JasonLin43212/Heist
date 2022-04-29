using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Player player;
    public float moveSpeed = 10f;

    public GameObject defaultItem;

    public GameObject shoutImage;
    // Parameters
    [Min(0f)]
    public float guardDistractTime = 0.2f;
    [Min(0f)]
    public float noiseRange = 5f;

    private Vector2 movement;
    public string x_axis;
    public string y_axis;

    // Controls
    private KeyCode pickDropKey;
    public bool pickItemsAutomatically = false;

    // Misc
    private ContactFilter2D itemContactFilter;
    private Collider2D myCollider;

    private GameObject justDroppedItemObject;

    // References
    public GameObject rangeDisplayObject;
    private ItemManager itemManager;
    public Animator animator;
    public AudioSource walkingSound;

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

        myCollider = GetComponent<Collider2D>();
        itemManager = GameState.Instance.ItemManager;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
        movement.x = Input.GetAxisRaw(x_axis);
        movement.y = Input.GetAxisRaw(y_axis);

        // 0: idle, 1: up, 2: right, 3: down, 4: left
        int movementId = 0;
        if (movement.y > 0) { movementId = 1; }
        else if (movement.y < 0) { movementId = 3; }
        else if (movement.x > 0) { movementId = 2; }
        else if (movement.x < 0) { movementId = 4; }
        animator.SetInteger("WalkDirection", movementId);

        if (movementId != 0) {
            if (!walkingSound.isPlaying) {
                walkingSound.Play();
            } 
        } else {
            walkingSound.Stop();
        }

        HandleItemPick();
        HandleItemUse();
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
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<BoxCollider2D>(), myCollider);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!pickItemsAutomatically) return;
        GameObject itemObject = other.transform.parent.gameObject;
        if (itemObject.tag == "Item")
        {
            if (itemObject == justDroppedItemObject)
            {
                justDroppedItemObject = null;
                return;
            }
            if (itemManager.PlayerIsHoldingItem(player))
            {
                ItemBehaviour heldItem = itemManager.GetHeldItemBehaviour(player);
                justDroppedItemObject = heldItem.gameObject;
                heldItem.Drop();
            }
            itemObject.GetComponent<ItemBehaviour>().Pickup(player);
        }
    }

    private void Shout()
    {
        Vector2 playerPosition = (Vector2)transform.position;

        // Creates a soundwave and sets its position
        GameObject currentShout = Instantiate(shoutImage);
        currentShout.GetComponent<Soundwave>().setPosition(playerPosition);
        // From the player position, it emits a shockwave that will hit all guards within range



        Collider2D playerRangeCollider = GetRangeCollider();
        SetRange(noiseRange);
        List<Collider2D> colliderResults = new List<Collider2D>();
        ContactFilter2D guardContactFilter = new ContactFilter2D();
        guardContactFilter.SetLayerMask(LayerMask.GetMask("Guard"));

        int numOverlappingColliders = playerRangeCollider.OverlapCollider(guardContactFilter, colliderResults);

        foreach (Collider2D guardCollider in colliderResults)
        {
            GameObject guardObject = guardCollider.gameObject;
            guardObject.GetComponent<GuardBehaviour>().DistractGuard(playerPosition, guardDistractTime);
        }
        HideRange();
    }

    private void HandleItemPick()
    {
        if (Input.GetKeyDown(pickDropKey))
        {
            // If currently holding an item, drop it
            if (itemManager.PlayerIsHoldingItem(player))
            {
                ItemBehaviour heldItem = itemManager.GetHeldItemBehaviour(player);
                justDroppedItemObject = heldItem.gameObject;
                heldItem.Drop();
            }

            // Look to see if we're touching any items
            Collider2D[] colliderBuffer = new Collider2D[GameConfig.ITEM_COLLIDER_BUFFER_SIZE];
            int numOverlappingColliders = GetComponent<Collider2D>().OverlapCollider(itemContactFilter, colliderBuffer);

            // If we are overlapping any other item, pick it up
            if (numOverlappingColliders > 0)
            {
                // Try to pick up the item
                GameObject overlappingItem = colliderBuffer[0].transform.parent.gameObject;
                overlappingItem.GetComponent<ItemBehaviour>().Pickup(player);  // is true if pick up was successful
            }
        }
    }

    private void HandleItemUse()
    {
        if (itemManager.PlayerIsHoldingItem(player))
        {
            // Check if item is being used
            ItemBehaviour heldItem = itemManager.GetHeldItemBehaviour(player);
            heldItem.CheckForUse(player);
        }
        else
        {
            if (Input.GetKeyDown(GameState.Instance.GetUseKey(player)))
                Shout();
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

    public Collider2D GetCollider() => myCollider;

    // Save/load tools
    public Vector2 Serialize()
    {
        return (Vector2)transform.position;
    }

    public void Deserialize(Vector2 state)
    {
        Vector2 playerPosition = state;
        transform.position = new Vector3(playerPosition.x, playerPosition.y, 0f);
    }
}
