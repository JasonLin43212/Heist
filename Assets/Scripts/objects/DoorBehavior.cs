using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    public bool closed = true;
    public BoxCollider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown(){
        if(closed){
            closed = false;
            this.transform.position += transform.up*.4f + transform.right*.5f;
            this.transform.rotation *= Quaternion.Euler(0,0,90);
        }else{
            closed = true;
            // this.transform.forward += new Vector3(-.5f,-.4f,0f);
            this.transform.position += transform.up*.5f + transform.right*-.4f;
            this.transform.rotation *= Quaternion.Euler(0,0,-90);
            ResetCollision();
        }
    }

    private void ResetCollision(){
        PlayerMovement[] players = (PlayerMovement[])FindObjectsOfType(typeof(PlayerMovement));
            foreach(PlayerMovement player in players){
                Physics2D.IgnoreCollision(collider, player.collider, false);
            }
    }
}
