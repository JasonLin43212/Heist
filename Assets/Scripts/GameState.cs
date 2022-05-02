using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState
{
    private static GameState instance;  // Single instance
    public static GameState Instance => instance;

    private GameObject controllerObject;
    private GameController gameController;
    private ClickController clickController;
    private CutsceneController cutsceneController;

    // Global GameObject references
    private GameObject player1Object, player2Object;
    private Camera player1Camera, player2Camera;
    private GameObject levelObject;

    // Variables
    public static string sceneName { get; set; }
    public int numberOfCamerasDisabled { get; set; }
    public bool payloadCollected { get; set; }
    public float stopwatchTime { get; set; }

    // Items
    private ItemManager itemManager;
    public ItemManager ItemManager => itemManager;

    // Controls
    private KeyCode[] pickDropKeys, useKeys;

    public GameState(
        GameObject controllerObject,
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
        this.controllerObject = controllerObject;
        gameController = controllerObject.GetComponent<GameController>();
        clickController = controllerObject.GetComponent<ClickController>();
        cutsceneController = controllerObject.GetComponent<CutsceneController>();

        this.player1Object = player1Object;
        this.player2Object = player2Object;
        this.player1Camera = player1Camera;
        this.player2Camera = player2Camera;
        this.levelObject = levelObject;

        this.pickDropKeys = new KeyCode[2] { player1PickDropKey, player2PickDropKey };
        this.useKeys = new KeyCode[2] { player1UseKey, player2UseKey };

        numberOfCamerasDisabled = 0;
        payloadCollected = false;
        stopwatchTime = 0;

        itemManager = new ItemManager();
        sceneName = SceneManager.GetActiveScene().name;

        instance = this;
    }

    // Getters
    public GameObject GetPlayerObject(Player player)
    {
        return (player == Player.Player1) ? player1Object : player2Object;
    }

    public PlayerMovement GetPlayerMovementScript(Player player)
    {
        return GetPlayerObject(player).GetComponent<PlayerMovement>();
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

    public GameController GameController => gameController;
    public ClickController ClickController => clickController;
    public CutsceneController CutsceneController => cutsceneController;


    // Save/load tools

    public (string sceneName, int camerasDisabled, bool payloadCollected) Serialize()
    {
        return (sceneName, numberOfCamerasDisabled, payloadCollected);
    }

    public void Deserialize((string, int, bool) state)
    {
        (string sceneName, int camerasDisabled, bool payloadCollected) = state;
        GameState.sceneName = sceneName;
        this.numberOfCamerasDisabled = camerasDisabled;
        this.payloadCollected = payloadCollected;
    }
}
