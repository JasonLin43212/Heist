using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Collider2D buttonCollider;

    public Color unpressedColor, pressedColor;

    private bool isTouchingPlayer = false;
    public bool IsTouchingPlayer => isTouchingPlayer;

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
        isTouchingPlayer = player1Rigidbody.IsTouching(buttonCollider) || player2Rigidbody.IsTouching(buttonCollider);
        spriteRenderer.material.color = isTouchingPlayer ? pressedColor : unpressedColor;
    }
}
