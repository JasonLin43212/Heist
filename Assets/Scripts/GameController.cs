using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // References
    public GameObject player1Object, player2Object;

    // Start is called before the first frame update
    void Start()
    {
        new GameState(
            player1Object: player1Object,
            player2Object: player2Object
        );
    }

    // Update is called once per frame
    void Update()
    {

    }
}
