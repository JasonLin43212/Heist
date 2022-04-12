using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    private static GameState instance;  // Single instance
    public static GameState Instance { get { return instance; } }

    // Global GameObject references
    private GameObject player1Object, player2Object;

    public GameState(
        GameObject player1Object,
        GameObject player2Object
    )
    {
        this.player1Object = player1Object;
        this.player2Object = player2Object;

        instance = this;
    }

    // Getters
    public GameObject GetPlayerObject(Player player)
    {
        return (player == Player.Player1) ? player1Object : player2Object;
    }
}
