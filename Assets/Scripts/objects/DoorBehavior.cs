using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    private const int RIGHT_ANGLE_DEGREES = 90;
    private const float VERTICAL_Y_DISPLACEMENT = .4f;
    private const float HORIZONTAL_X_DISPLACEMENT = .5f;

    private float initialRotation;
    private Vector3 initialPosition;
    private bool closed = true;
    private bool guardNotLooking = true;

    public bool shouldDoorBeClosed = true;
    public BoxCollider2D boxCollider;


    // Start is called before the first frame update
    void Start()
    {
        initialRotation = this.transform.rotation.eulerAngles.z;
        initialPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(guardNotLooking && closed){
            shouldDoorBeClosed = true;
            closeDoor();
            ResetCollision();
        }else{
            shouldDoorBeClosed = false;
            closeDoor();
        }

    }

    

    void OnMouseDown(){
        if(closed){
            closed = false;
        }else{
            closed = true;
        }
    }

    void OnTriggerStay2D(Collider2D collider){
        if(collider.gameObject.tag == "Guard"){
            guardNotLooking = false;
        }
    }

    void OnTriggerExit2D(Collider2D collider){
        if(collider.gameObject.tag == "Guard"){
            guardNotLooking = true;
        }
    }

    void closeDoor(){
        if(shouldDoorBeClosed){
            this.transform.rotation = Quaternion.Euler(0,0,initialRotation);
            this.transform.position = initialPosition;
        } else{
            this.transform.rotation = Quaternion.Euler(0,0,initialRotation-RIGHT_ANGLE_DEGREES);
            this.transform.position = initialPosition + transform.up*VERTICAL_Y_DISPLACEMENT + transform.right*HORIZONTAL_X_DISPLACEMENT;
        }
    }

    private void ResetCollision(){
        PlayerMovement[] players = (PlayerMovement[])FindObjectsOfType(typeof(PlayerMovement));
            foreach(PlayerMovement player in players){
                Physics2D.IgnoreCollision(boxCollider, player.circleCollider, false);
            }
    }
}
