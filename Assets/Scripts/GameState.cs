using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    private static GameState instance;  // Single instance
    public static GameState Instance => instance;

    // Global GameObject references
    private GameObject player1Object, player2Object;

    // Items
    private ItemManager itemManager;
    public ItemManager ItemManager => itemManager;

    // Controls
    private KeyCode[] pickDropKeys, useKeys;

    public GameState(
        GameObject player1Object,
        GameObject player2Object,
        // Controls
        KeyCode player1PickDropKey = KeyCode.C,
        KeyCode player2PickDropKey = KeyCode.RightShift,
        KeyCode player1UseKey = KeyCode.V,
        KeyCode player2UseKey = KeyCode.Return
    )
    {
        this.player1Object = player1Object;
        this.player2Object = player2Object;
        this.pickDropKeys = new KeyCode[2] { player1PickDropKey, player2PickDropKey };
        this.useKeys = new KeyCode[2] { player1UseKey, player2UseKey };

        itemManager = new ItemManager();

        instance = this;
    }

    // Getters
    public GameObject GetPlayerObject(Player player)
    {
        return (player == Player.Player1) ? player1Object : player2Object;
    }

    public KeyCode GetPickDropKey(Player player)
    {
        return pickDropKeys[(int)player];
    }

    public KeyCode GetUseKey(Player player)
    {
        return useKeys[(int)player];
    }
}
