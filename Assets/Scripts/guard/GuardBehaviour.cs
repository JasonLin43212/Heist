using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBehaviour : MonoBehaviour
{
    // Constants
    const float MOVE_SPEED = 1.5f, ROTATION_SPEED = 180f, ANGLE_TOL = 1f, POSITION_TOL = 0.05f;

    // Movement variables
    public List<Vector2> defaultRoute;
    private int defaultRouteIndex, queueIndex;
    private List<Vector2> queue;
    private Vector2 targetPosition;
    private float targetAngle;

    // References
    private Rigidbody2D myRigidbody;


    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        defaultRouteIndex = 0;
        queueIndex = 0;
        queue = new List<Vector2>();

        UpdateMoveTargets();
    }

    void FixedUpdate()
    {
        float angleDifference = targetAngle % 360 - myRigidbody.rotation % 360;
        if (Mathf.Abs(angleDifference) >= ANGLE_TOL)
        {
            // Rotate
            float rotationDelta = Time.fixedDeltaTime * ((angleDifference > 0) ? ROTATION_SPEED : -ROTATION_SPEED);
            myRigidbody.MoveRotation(myRigidbody.rotation + rotationDelta);
        }
        else if (Vector2.Distance(myRigidbody.position, targetPosition) >= POSITION_TOL)
        {
            // Move
            Vector2 movementDirection = targetPosition - myRigidbody.position;
            movementDirection /= movementDirection.magnitude;
            myRigidbody.MovePosition(myRigidbody.position + movementDirection * MOVE_SPEED * Time.fixedDeltaTime);
        }
        else UpdateMoveTargets();
    }

    // Updates the target position and angle of the guard based on its route
    private void UpdateMoveTargets()
    {
        if (queueIndex >= queue.Count || queue.Count == 0)
        {
            // Set queue to next default waypoint if it is empty or finished
            queue = new List<Vector2>();
            queue.Add(defaultRoute[defaultRouteIndex]);
            defaultRouteIndex = (defaultRouteIndex + 1) % defaultRoute.Count;
            queueIndex = 0;
        }

        targetPosition = queue[queueIndex];
        targetAngle = Mathf.Atan2(targetPosition.y - myRigidbody.position.y, targetPosition.x - myRigidbody.position.x) * Mathf.Rad2Deg;
        queueIndex++;
    }
}
