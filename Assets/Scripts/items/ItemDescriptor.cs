using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemDescriptor : MonoBehaviour
{
    public abstract string Name { get; }

    public Sprite displaySprite;

    // Item use interfaces
    public virtual bool CanUseWithKey => false;
    protected virtual float ItemUseRange => 0f;

    public bool showPlayerRangeCircle = true;

    protected ItemBehaviour itemBehaviour;

    protected virtual void Start()
    {
        itemBehaviour = GetComponent<ItemBehaviour>();
    }

    protected virtual void Update() { }

    public virtual bool UseKeyPressed() => false;
    public virtual void OnPickup(Player player)
    {
        GameObject playerObject = GameState.Instance.GetPlayerObject(player);
        playerObject.GetComponent<PlayerMovement>().SetRange(ItemUseRange);
        if (!showPlayerRangeCircle) playerObject.GetComponent<PlayerMovement>().rangeDisplayObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    public virtual void OnDrop(Player player)
    {
        GameObject playerObject = GameState.Instance.GetPlayerObject(player);
        playerObject.GetComponent<PlayerMovement>().HideRange();
        if (!showPlayerRangeCircle) playerObject.GetComponent<PlayerMovement>().rangeDisplayObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    // Getters + Utility functions

    public Sprite GetDisplaySprite() => displaySprite;

    protected virtual Collider2D GetHolderRangeCollider()
    {
        Player? holder = itemBehaviour.GetHolder();
        if (!holder.HasValue) return null;
        GameObject holderObject = GameState.Instance.GetPlayerObject(holder.Value);
        return holderObject.GetComponent<PlayerMovement>().GetRangeCollider();
    }
}
