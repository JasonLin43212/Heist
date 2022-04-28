using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GoalTriggerScript : MonoBehaviour
{
    private Collider2D myCollider;
    private Rigidbody2D player1Rigidbody, player2Rigidbody;

    public bool isPayloadTrigger = false;
    public bool isVictoryTrigger = false;

    // Start is called before the first frame update
    void Start()
    {
        player1Rigidbody = GameState.Instance.GetPlayerObject(Player.Player1).GetComponent<Rigidbody2D>();
        player2Rigidbody = GameState.Instance.GetPlayerObject(Player.Player2).GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        // transform.GetChild(0).gameObject.SetActive(false);  // Hide sprite
    }

    void Update()
    {
        if (GetComponent<CutsceneTriggerScript>() != null || PauseMenu.PausedByCutscene) return;
        if ((player1Rigidbody.IsTouching(myCollider) || player2Rigidbody.IsTouching(myCollider)) && isPayloadTrigger)  // change to && if you want both players
        {
            GameState.Instance.payloadCollected = true;
            Destroy(gameObject);
        } else if (player1Rigidbody.IsTouching(myCollider) && player2Rigidbody.IsTouching(myCollider) && isVictoryTrigger)
        {
            // Win the game!
            SavedState.hasSavedContent = false;
            SceneManager.LoadScene(sceneName: "Win Screen");
            Destroy(gameObject);
        }
    }
}
