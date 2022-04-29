using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : ItemBehaviour
{
    public override ItemType ItemType => ItemType.Consumable;
    public override bool CanHold => true;
    public override bool HasFiniteUses => true;

    public bool destroyAfterBeingUsedUp = false;
    public override bool DestroyAfterUsedUp => destroyAfterBeingUsedUp;
    public int maximumUses, startingUses;

    protected override void InitializeItemProperties()
    {
        base.InitializeItemProperties();
        maxUses = maximumUses;
        remainingUses = startingUses;
    }
}
