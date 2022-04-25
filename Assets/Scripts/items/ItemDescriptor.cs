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

    protected ItemBehaviour itemBehaviour;

    protected virtual void Start()
    {
        itemBehaviour = GetComponent<ItemBehaviour>();
    }

    protected virtual void Update() { }

    public virtual bool UseKeyPressed() => false;
    public virtual void OnPickup(Player player)
    {
        GameState.Instance.GetPlayerObject(player).GetComponent<PlayerMovement>().SetRange(ItemUseRange);
    }

    public virtual void OnDrop(Player player)
    {
        GameState.Instance.GetPlayerObject(player).GetComponent<PlayerMovement>().HideRange();
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
