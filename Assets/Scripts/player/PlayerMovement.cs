using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Player player;
    public float moveSpeed = 10f;

    private Vector2 movement;
    public string x_axis;
    public string y_axis;
    public CircleCollider2D circleCollider;
    // Start is called before the first frame update
    void Start()
    {
        if (player == Player.Player1)
        {
            x_axis = "Horizontal";
            y_axis = "Vertical";
        }
        else
        {
            x_axis = "Horizontal2";
            y_axis = "Vertical2";
        }

    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw(x_axis);
        movement.y = Input.GetAxisRaw(y_axis);
    }

    void FixedUpdate()
    {
        Rigidbody2D myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.MovePosition(myRigidbody.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
    
    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Door" && collision.gameObject.GetComponent<DoorBehavior>().shouldDoorBeClosed == false){
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<BoxCollider2D>(), circleCollider);
        }
    }
}
