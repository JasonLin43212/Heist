using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private const int RIGHT_ANGLE_DEGREES = 90;
    private const float VERTICAL_Y_DISPLACEMENT = .4f;
    private const float HORIZONTAL_X_DISPLACEMENT = .5f;

    private float initialRotation;
    private Vector3 initialPosition;
    protected bool closed = true;
    private bool guardNotLooking = true;
    private bool doorWasAlreadyClosed = true;

    public bool shouldDoorBeClosed = true, openableByGuard = true;
    protected bool defaultState;
    public Transform spriteTransform;
    public BoxCollider2D boxCollider;


    // Start is called before the first frame update
    void Start()
    {
        initialRotation = spriteTransform.rotation.eulerAngles.z;
        initialPosition = spriteTransform.position;
        defaultState = shouldDoorBeClosed;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (guardNotLooking && closed)
        {
            shouldDoorBeClosed = true;
            if (!doorWasAlreadyClosed)
            {
                closeDoor();
                ResetCollision();
                doorWasAlreadyClosed = true;
            }
        }
        else
        {
            shouldDoorBeClosed = false;
            if (doorWasAlreadyClosed)
            {
                closeDoor();
                doorWasAlreadyClosed = false;
            }
        }
    }

    void closeDoor()
    {
        if (shouldDoorBeClosed)
        {
            spriteTransform.rotation = Quaternion.Euler(0, 0, initialRotation);
            spriteTransform.position = initialPosition;
        }
        else
        {
            spriteTransform.rotation = Quaternion.Euler(0, 0, initialRotation - RIGHT_ANGLE_DEGREES);
            spriteTransform.position = initialPosition + spriteTransform.up * VERTICAL_Y_DISPLACEMENT + spriteTransform.right * HORIZONTAL_X_DISPLACEMENT;
        }
    }

    private void ResetCollision()
    {
        PlayerMovement[] players = (PlayerMovement[])FindObjectsOfType(typeof(PlayerMovement));
        foreach (PlayerMovement player in players)
        {
            Physics2D.IgnoreCollision(boxCollider, player.GetCollider(), false);
        }
    }

    public void SetGuardNotLooking(bool target)
    {
        if (openableByGuard) guardNotLooking = target;
    }
}
