using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBehaviour : MonoBehaviour
{
    public abstract ItemType ItemType { get; }

    // Resources / References
    public GameObject spriteObject;  // Child GameObject containing the item's sprite and collider
    private ItemDescriptor itemDescriptor;

    // Holding the item
    public virtual bool CanHold => false;
    protected Player? holder = null;

    // Using the item
    public virtual bool HasFiniteUses => false;
    public virtual bool DestroyAfterUsedUp => false;
    protected int maxUses = 0, remainingUses = 0;

    private string uniqueIdentifier;
    public string UniqueID => uniqueIdentifier;

    protected virtual void Start()
    {
        itemDescriptor = GetComponent<ItemDescriptor>();
        InitializeItemProperties();
        ValidateItemProperties();
        uniqueIdentifier = $"Item<{itemDescriptor.Name},{transform.position.ToString()}>";
    }

    protected virtual void Update()
    {
        if (CanHold)
        {
            if (holder.HasValue)
            {
                // Keep item updated to where its holder is located
                GameObject holderObject = GameState.Instance.GetPlayerObject(holder.Value);
                transform.position = holderObject.transform.position;
                // Hide the map sprite
                spriteObject.SetActive(false);
            }
            else spriteObject.SetActive(true);
        }
    }

    // Item pickup / drop methods

    // Attempt to pick up the item. Returns true iff successful.
    public virtual bool Pickup(Player player)
    {
        if (!CanHold) return false;  // item is not holdable
        if (holder.HasValue) return false;  // item already being held
        holder = player;
        GameState.Instance.ItemManager.SetHeldItem(player, gameObject);
        itemDescriptor.OnPickup(player);
        return true;
    }

    public virtual void Drop()
    {
        if (holder.HasValue)
        {
            Player prevPlayer = holder.Value;
            GameState.Instance.ItemManager.RemoveHeldItem(prevPlayer);
            holder = null;
            itemDescriptor.OnDrop(prevPlayer);
        }
    }

    // Item use methods
    // Bool methods return true if item was successfully used, otherwise false

    // Checks for user input and calls use methods if appropriate - this method should be repeatedly executed whenever held
    public virtual bool CheckForUse(Player player)
    {
        if (itemDescriptor == null)
        {
            Debug.Log("No attached item descriptor!");
            return false;  // No attached item descriptor!
        }

        if (HasFiniteUses && remainingUses < 1)
        {
            // Item is out of uses.
            return false;
        }

        // Use key pressed
        if (itemDescriptor.CanUseWithKey && Input.GetKeyDown(GameState.Instance.GetUseKey(player)))
        {
            if (itemDescriptor.UseKeyPressed())
            {
                RecordUse();
                return true;
            }
        }

        return false;
    }

    // Called each time this item is used via any means
    protected virtual void RecordUse()
    {
        if (HasFiniteUses)
        {
            remainingUses--;
            if (remainingUses == 0) ItemOutOfUses();
        }
    }

    // Called when a finite-use item runs out of uses
    protected virtual void ItemOutOfUses()
    {
        Debug.Log("Item ran out of uses.");
        if (DestroyAfterUsedUp) Destroy(gameObject);
    }

    // Set all important starting fields
    protected virtual void InitializeItemProperties() { }

    private void ValidateItemProperties()
    {
        Debug.Assert(HasFiniteUses || (maxUses == 0 && remainingUses == 0), "If HasFiniteUses is false, maxUses and remainingUses must be set to 0.");
    }

    // Getters
    public bool IsBeingHeld() => holder.HasValue;
    public Player? GetHolder() => holder;
    public int GetRemainingUses() => remainingUses;
    public int GetMaxUses() => maxUses;

    // Save / load methods
    public (int remainingUses, int maxUses, Vector3 location) Serialize()
    {
        Vector3 myLocation = (CanHold && holder.HasValue) ? GameState.Instance.GetPlayerObject(holder.Value).transform.position : transform.position;
        return (remainingUses, maxUses, myLocation);
    }

    public void Deserialize((int, int, Vector3) state)
    {
        (int remainingUses, int maxUses, Vector3 location) = state;
        this.remainingUses = remainingUses;
        this.maxUses = maxUses;
        transform.position = location;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
