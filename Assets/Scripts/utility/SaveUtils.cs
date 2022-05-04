using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SavedState
{
    public static bool hasSavedContent = false;
    public static (string, int, bool) gameStateState;
    public static Vector2 player1State, player2State;
    public static Dictionary<string, GuardState> guardStates;
    public static Dictionary<string, (int, int, Vector3)> itemStates;
    public static Dictionary<string, MouseCameraState> cameraStates;
    public static Dictionary<string, (bool, bool, bool, string)> doorStates;
    public static HashSet<string> existingTriggers;
    public static float gameStopwatchTime;
}


public class SaveUtils : MonoBehaviour
{
    void Start()
    {
        if (SavedState.hasSavedContent)
        {
            LoadCurrentState();
            LoadStopwatchTime();
        }
    }

    public static void SaveCurrentState()
    {
        SavedState.hasSavedContent = true;
        SavedState.gameStateState = GameState.Instance.Serialize();
        SavedState.player1State = GameState.Instance.GetPlayerMovementScript(Player.Player1).Serialize();
        SavedState.player2State = GameState.Instance.GetPlayerMovementScript(Player.Player2).Serialize();

        // Guards
        SavedState.guardStates = new Dictionary<string, GuardState>();
        foreach (GuardBehaviour obj in FindObjectsOfType<GuardBehaviour>())
        {
            SavedState.guardStates[obj.UniqueID] = obj.Serialize();
        }

        // Items
        SavedState.itemStates = new Dictionary<string, (int, int, Vector3)>();
        foreach (ItemBehaviour obj in FindObjectsOfType<ItemBehaviour>())
        {
            SavedState.itemStates[obj.UniqueID] = obj.Serialize();
        }

        // Cameras
        SavedState.cameraStates = new Dictionary<string, MouseCameraState>();
        foreach (BasicMouseCamera obj in FindObjectsOfType<BasicMouseCamera>())
        {
            SavedState.cameraStates[obj.UniqueID] = obj.Serialize();
        }

        // Doors
        SavedState.doorStates = new Dictionary<string, (bool, bool, bool, string)>();
        foreach (DoorScript obj in FindObjectsOfType<DoorScript>())
        {
            SavedState.doorStates[obj.UniqueID] = obj.Serialize();
        }

        // Guards
        SavedState.existingTriggers = new HashSet<string>();
        foreach (GenericTriggerScript obj in FindObjectsOfType<GenericTriggerScript>())
        {
            SavedState.existingTriggers.Add(obj.UniqueID);
        }
    }

    public static void LoadCurrentState()
    {
        GameState.Instance.Deserialize(SavedState.gameStateState);
        GameState.Instance.GetPlayerMovementScript(Player.Player1).Deserialize(SavedState.player1State);
        GameState.Instance.GetPlayerMovementScript(Player.Player2).Deserialize(SavedState.player2State);

        // Guards
        foreach (GuardBehaviour obj in FindObjectsOfType<GuardBehaviour>())
        {
            obj.Deserialize(SavedState.guardStates[obj.UniqueID]);
        }

        // Items
        foreach (ItemBehaviour obj in FindObjectsOfType<ItemBehaviour>())
        {
            if (!SavedState.itemStates.ContainsKey(obj.UniqueID)) obj.DestroySelf();
            else obj.Deserialize(SavedState.itemStates[obj.UniqueID]);
        }

        // Cameras
        foreach (BasicMouseCamera obj in FindObjectsOfType<BasicMouseCamera>())
        {
            obj.Deserialize(SavedState.cameraStates[obj.UniqueID]);
        }

        // Doors
        foreach (DoorScript obj in FindObjectsOfType<DoorScript>())
        {
            obj.Deserialize(SavedState.doorStates[obj.UniqueID]);
        }

        // Triggers
        foreach (GenericTriggerScript obj in FindObjectsOfType<GenericTriggerScript>())
        {
            if (!SavedState.existingTriggers.Contains(obj.UniqueID)) obj.DestroySelf();
        }
    }

    public static void SaveStopwatchTime()
    {
        SavedState.gameStopwatchTime = GameState.Instance.stopwatchTime;
    }

    public static void LoadStopwatchTime()
    {
        GameState.Instance.stopwatchTime = SavedState.gameStopwatchTime;
    }
}
