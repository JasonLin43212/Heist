using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PermanentItem : Item
{
    public override ItemType ItemType => ItemType.Permanent;
    public override bool HasFiniteUses => true;
}
