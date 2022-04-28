using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (player1Rigidbody.IsTouching(myCollider) || player2Rigidbody.IsTouching(myCollider))  // change to && if you want both players
        {
            if (GetComponent<CutsceneTriggerScript>() != null || PauseMenu.PausedByCutscene) return;
            if (isPayloadTrigger) GameState.Instance.payloadCollected = true;
            else if (isVictoryTrigger)
            {
                // Win the game!
                Debug.Log("game won");
            }
            Destroy(gameObject);
        }
    }
}
