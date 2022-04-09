using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;

    public Rigidbody2D rigidbody;

    Vector2 movement;
    public string x_axis;
    public string y_axis;
    // Start is called before the first frame update
    void Start()
    {
        if(this.name == "Player 1"){
            x_axis = "Horizontal";
            y_axis = "Vertical";
        } else{
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
        rigidbody.MovePosition(rigidbody.position + movement*moveSpeed*Time.fixedDeltaTime);
    }
}
