using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameController : MonoBehaviour
{
    // References
    public GameObject player1Object, player2Object;
    private Camera player1Camera, player2Camera;
    public GameObject levelObject;
    public TMP_Text gameStopwatch;

    private bool isStopwatchCounting;

    // Cheat codes
    public bool disableDeath;

    // Start is called before the first frame update
    void Start()
    {
        // Get cameras
        player1Camera = player1Object.GetComponentInChildren<Camera>(true);
        player2Camera = player2Object.GetComponentInChildren<Camera>(true);

        new GameState(
            controllerObject: gameObject,
            player1Object: player1Object,
            player2Object: player2Object,
            player1Camera: player1Camera,
            player2Camera: player2Camera,
            levelObject: levelObject
        );

        SpawnPlayersOnLevel();

        isStopwatchCounting = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isStopwatchCounting)
        {
            GameState.Instance.stopwatchTime += Time.deltaTime;
            gameStopwatch.text = TimeSpan.FromSeconds(GameState.Instance.stopwatchTime).ToString(@"mm\:ss\.ff");
        } 
    }

    public void PlayerCaught(Player caughtPlayer)
    {
        Debug.Log($"Player {(int)caughtPlayer + 1} was caught!");
        if (!disableDeath){
            SaveUtils.SaveStopwatchTime();
            SceneManager.LoadScene(sceneName: "Lose Screen");
        }
    }

    private void SpawnPlayersOnLevel()
    {
        LevelBehaviour levelBehaviourScript = levelObject.GetComponent<LevelBehaviour>();
        player1Object.SetActive(true);
        player1Object.transform.position = levelBehaviourScript.GetPlayerSpawnPosition(Player.Player1);
        player2Object.SetActive(true);
        player2Object.transform.position = levelBehaviourScript.GetPlayerSpawnPosition(Player.Player2);
    }

    public void resumeStopwatch(){
        isStopwatchCounting = true;
    }

    public void pauseStopwatch(){
        isStopwatchCounting = false;
    }
}
