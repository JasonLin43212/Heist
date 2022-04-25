using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHUDScript : MonoBehaviour
{
    public Player player;

    private ItemManager itemManager;

    public TMP_Text itemNameText;
    public TMP_Text itemDurabilityText;

    // Start is called before the first frame update
    void Start()
    {
        itemManager = GameState.Instance.ItemManager;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateItemUI();
    }

    // Display UI elements to show the item being held
    // TODO: IMPLEMENT
    private void UpdateItemUI()
    {
        // (From Lilian) here's the interface which should give you all the available information you need
        // Is the player holding an item?
        bool isHoldingItem = itemManager.PlayerIsHoldingItem(player);
        if (isHoldingItem)
        {
            // Each item has two scripts (ItemBehaviour and ItemDescriptor) associated with it which you can get info from
            ItemBehaviour itemBehaviour = itemManager.GetHeldItemBehaviour(player);
            ItemDescriptor itemDescriptor = itemManager.GetHeldItemDescriptor(player);

            // ItemBehaviour has information about the state of the item
            // ItemType itemType = itemBehaviour.ItemType;  // this is an enum... not actually sure if you need this or not but whatevs
            bool itemHasFiniteUses = itemBehaviour.HasFiniteUses;  // this is true if the item has a finite number of uses;
                                                                   // if false, it can be used unlimited times
            int remainingUses = itemBehaviour.GetRemainingUses();  // number of uses remaining (0 if itemHasFiniteUses is false)
            // int maxUses = itemBehaviour.GetMaxUses();  // maximum number of stored uses, i.e. if we ever have an item that can replenish (idk if you want this)

            // ItemDescriptor has information about the type of item
            string itemName = itemDescriptor.Name;  // e.g. "Crowbar"
            // bool doesUseKeyWork = itemDescriptor.CanUseWithKey;  // true iff it responds to the "use key" (V or Enter/Return)
            // Sprite displaySprite = itemDescriptor.GetDisplaySprite();  // returns the sprite which you can attach via the editor

            // TODO pls display something lol

            itemNameText.text = itemName;
            itemDurabilityText.text = itemHasFiniteUses ? remainingUses.ToString() : "∞";
        }
        else
        {
            // TODO if you want to display anything when you're not holding an item
            itemNameText.text = "None";
            itemDurabilityText.text = "∞";
        }
    }
}
