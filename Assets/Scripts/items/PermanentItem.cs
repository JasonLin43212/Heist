using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentItem : ItemBehaviour
{
    public override ItemType ItemType => ItemType.Permanent;
    public override bool CanHold => true;
    public override bool HasFiniteUses => true;
}
