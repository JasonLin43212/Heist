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
    protected int totalUses = 0, remainingUses = 0;

    protected virtual void Start()
    {
        itemDescriptor = GetComponent<ItemDescriptor>();
        InitializeItemProperties();
        ValidateItemProperties();
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
        return true;
    }

    public virtual void Drop()
    {
        if (holder.HasValue)
        {
            GameState.Instance.ItemManager.RemoveHeldItem(holder.Value);
            holder = null;
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
    protected virtual void ItemOutOfUses() { }

    // Set all important starting fields
    protected virtual void InitializeItemProperties() { }

    private void ValidateItemProperties()
    {
        Debug.Assert(HasFiniteUses || (totalUses == 0 && remainingUses == 0), "If HasFiniteUses is false, totalUses and remainingUses must be set to 0.");
    }

    // Getters
    public bool IsBeingHeld() => holder.HasValue;
    public Player? GetHolder() => holder;
    public int GetRemainingUses() => remainingUses;
    public int GetTotalUses() => totalUses;
}
