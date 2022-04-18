using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    private static GameState instance;  // Single instance
    public static GameState Instance { get { return instance; } }

    private GameController gameController;
    public GameController ControllerScript { get { return gameController; } }

    // Global GameObject references
    private GameObject player1Object, player2Object;
    private GameObject levelObject;
    public static string sceneName { get; set; }

    public GameState(
        GameController gameController,
        GameObject player1Object,
        GameObject player2Object,
        GameObject levelObject
    )
    {
        this.gameController = gameController;
        this.player1Object = player1Object;
        this.player2Object = player2Object;
        this.levelObject = levelObject;

        instance = this;
    }

    // Getters
    public GameObject GetPlayerObject(Player player)
    {
        return (player == Player.Player1) ? player1Object : player2Object;
    }

}
