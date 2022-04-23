using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    private static GameState instance;  // Single instance
    public static GameState Instance => instance;

    private GameController gameController;
    public GameController ControllerScript { get { return gameController; } }

    private ClickController clickController;
    public ClickController ClickControllerScript { get { return clickController; } }

    // Global GameObject references
    private GameObject player1Object, player2Object;
    private Camera player1Camera, player2Camera;
    private GameObject levelObject;
    public static string sceneName { get; set; }
    public int numberOfCamerasDisabled { get; set; }

    // Items
    private ItemManager itemManager;
    public ItemManager ItemManager => itemManager;

    // Controls
    private KeyCode[] pickDropKeys, useKeys;

    public GameState(
        GameController gameController,
        ClickController clickController,
        GameObject player1Object,
        GameObject player2Object,
        Camera player1Camera,
        Camera player2Camera,
        GameObject levelObject,
        // Controls
        KeyCode player1PickDropKey = KeyCode.C,
        KeyCode player2PickDropKey = KeyCode.RightShift,
        KeyCode player1UseKey = KeyCode.V,
        KeyCode player2UseKey = KeyCode.Return
    )
    {
        this.gameController = gameController;
        this.clickController = clickController;
        this.player1Object = player1Object;
        this.player2Object = player2Object;
        this.player1Camera = player1Camera;
        this.player2Camera = player2Camera;
        this.levelObject = levelObject;

        this.pickDropKeys = new KeyCode[2] { player1PickDropKey, player2PickDropKey };
        this.useKeys = new KeyCode[2] { player1UseKey, player2UseKey };

        numberOfCamerasDisabled = 0;

        itemManager = new ItemManager();

        instance = this;
    }

    // Getters
    public GameObject GetPlayerObject(Player player)
    {
        return (player == Player.Player1) ? player1Object : player2Object;
    }

    public Camera GetPlayerCamera(Player player)
    {
        return (player == Player.Player1) ? player1Camera : player2Camera;
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
