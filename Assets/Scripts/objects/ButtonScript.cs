using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Collider2D buttonCollider;

    public GameObject guard; // the guard that may step on the button

    public Color unpressedColor, pressedColor;

    private bool isTouchingPlayer = false;
    public bool IsTouchingPlayer => isTouchingPlayer;
    private bool wasTouchingPlayer;
    public AudioSource pressSound;

    private Rigidbody2D player1Rigidbody, player2Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        player1Rigidbody = GameState.Instance.GetPlayerObject(Player.Player1).GetComponent<Rigidbody2D>();
        player2Rigidbody = GameState.Instance.GetPlayerObject(Player.Player2).GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        wasTouchingPlayer = isTouchingPlayer;

        isTouchingPlayer = player1Rigidbody.IsTouching(buttonCollider) || player2Rigidbody.IsTouching(buttonCollider);
        
        if (!wasTouchingPlayer && isTouchingPlayer) {
            pressSound.Play();
        }
        // Also check if the guard is touching the button, if it exists
        if(guard != null) 
        {
            GameObject collisionBox = guard.transform.GetChild(3).gameObject; 
            isTouchingPlayer |= collisionBox.GetComponent<BoxCollider2D>().IsTouching(buttonCollider);
            // isTouchingPlayer |= guard.GetComponent<Rigidbody2D>().IsTouching(buttonCollider);
        }

        
        spriteRenderer.material.color = isTouchingPlayer ? pressedColor : unpressedColor;
    }
}
