using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // References
    public GameObject player1Object, player2Object;
    public GameObject levelObject;

    // Cheat codes
    public bool disableDeath;

    // Start is called before the first frame update
    void Start()
    {
        new GameState(
            gameController: this,
            player1Object: player1Object,
            player2Object: player2Object,
            levelObject: levelObject
        );

        SpawnPlayersOnLevel();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayerCaught(Player caughtPlayer)
    {
        Debug.Log($"Player {(int)caughtPlayer + 1} was caught!");
        if (!disableDeath) SceneManager.LoadScene(sceneName: "Lose Screen");
    }

    private void SpawnPlayersOnLevel()
    {
        LevelBehaviour levelBehaviourScript = levelObject.GetComponent<LevelBehaviour>();
        player1Object.SetActive(true);
        player1Object.transform.position = levelBehaviourScript.GetPlayerSpawnPosition(Player.Player1);
        player2Object.SetActive(true);
        player2Object.transform.position = levelBehaviourScript.GetPlayerSpawnPosition(Player.Player2);
    }
}
