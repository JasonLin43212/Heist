using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCrowbar : ItemDescriptor
{
    public override string Name => "Crowbar";
    public override ItemType ItemType => ItemType.Permanent;

    public override bool CanUseWithKey => true;

    public override bool UseKeyPressed()
    {
        Debug.Log($"Crowbar used by {itemBehaviour.GetHolder()}.");
        return true;
    }
}
