using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager
{
    // Player held items
    private GameObject player1Item, player2Item;

    // Handle held items
    public void SetHeldItem(Player player, GameObject itemObject)
    {
        // Error checking
        if (itemObject && itemObject.GetComponent<ItemBehaviour>() == null)
            throw new System.Exception("Attempted to hold an item that doesn't have an Item script.");
        if (itemObject && itemObject.GetComponent<ItemBehaviour>().GetHolder() != player)
            throw new System.Exception("Item's stored holder is inconsistent with argument passed into SetHeldItem.");

        if (player == Player.Player1) player1Item = itemObject;
        else player2Item = itemObject;
    }

    public void RemoveHeldItem(Player player)
    {
        if (player == Player.Player1) player1Item = null;
        else player2Item = null;
    }

    public GameObject GetHeldItemObject(Player player)
    {
        return (player == Player.Player1) ? player1Item : player2Item;
    }

    public ItemBehaviour GetHeldItem(Player player)
    {
        GameObject heldItemObject = GetHeldItemObject(player);
        return heldItemObject.GetComponent<ItemBehaviour>();
    }

    public ItemDescriptor GetHeldItemDescriptor(Player player)
    {
        GameObject heldItemObject = GetHeldItemObject(player);
        return heldItemObject.GetComponent<ItemDescriptor>();
    }

    public bool PlayerIsHoldingItem(Player player) => GetHeldItemObject(player) != null;
}
