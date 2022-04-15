using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemDescriptor : MonoBehaviour
{
    public abstract string Name { get; }
    public abstract ItemType ItemType { get; }

    public Sprite displaySprite;

    // Item use interfaces
    public virtual bool CanUseWithKey => false;

    protected ItemBehaviour itemBehaviour;

    protected virtual void Start()
    {
        itemBehaviour = GetComponent<ItemBehaviour>();
    }

    public virtual bool UseKeyPressed() => false;

    public Sprite GetDisplaySprite() => displaySprite;
}
