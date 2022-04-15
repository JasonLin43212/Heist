using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBehaviour : MonoBehaviour
{
    public GameObject player1SpawnMarker, player2SpawnMarker;

    void Start()
    {
        player1SpawnMarker.SetActive(false);
        player2SpawnMarker.SetActive(false);
    }

    public Vector3 GetPlayerSpawnPosition(Player player)
    {
        GameObject spawnMarker = (player == Player.Player1) ? player1SpawnMarker : player2SpawnMarker;
        Vector3 spawnPosition = spawnMarker.transform.position;
        return new Vector3(spawnPosition.x, spawnPosition.y, 0);
    }

    public Color GetPlayerColor(Player player)
    {
        GameObject spawnMarker = (player == Player.Player1) ? player1SpawnMarker : player2SpawnMarker;
        GameObject spawnSprite = spawnMarker.transform.GetChild(0).gameObject;
        return spawnSprite.GetComponent<SpriteRenderer>().color;
    }
}
