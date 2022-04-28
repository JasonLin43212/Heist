using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Collider2D buttonCollider;

    public GameObject guard; // the guard that may step on the button

    public Color unpressedColor, pressedColor;

    private bool isTouched = false;
    public bool isTouched => isTouched;

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
        isTouched = player1Rigidbody.IsTouching(buttonCollider) || player2Rigidbody.IsTouching(buttonCollider);
        // Also check if the guard's polygon collider is touching the button, if it exists
        if(guard != null) 
        {
            GameObject collisionBox = guard.transform.GetChild(3).gameObject;
            isTouched |= collisionBox.GetComponent<PolygonCollider2D>().IsTouching(buttonCollider);
        }
        spriteRenderer.material.color = isTouched ? pressedColor : unpressedColor;
    }
}
