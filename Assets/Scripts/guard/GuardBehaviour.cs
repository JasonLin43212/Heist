using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBehaviour : MonoBehaviour
{
    // Constants
    const float ROTATION_SPEED = 180f, ANGLE_TOL = 0.3f, POSITION_TOL = 0.05f;  // movement
    const float PLAYER_RADIUS = 0.4f;

    // Modifiable constants

    public float visionRange = 15f, visionAngle = 30f;
    public float targetChaseDistanceRatio = 0.7f;  // When pursuing a player, the guard tries to stay within visionRange * targetChaseDistanceRatio
    public float moveSpeed = 1.5f;

    public float chaseSpeed = 3f;
    public float secondsToCatch = 1.5f;
    public int visionConeResolution = 100;

    // Movement variables
    public List<GuardRouteAction> defaultRouteActions;
    private int defaultRouteIndex, queueIndex;
    private List<GuardRouteAction> queue;
    private Vector2 targetPosition;
    private float targetAngle;

    private enum MovementMode { Default, LookLeft, LookRight, Backtrack };
    private MovementMode movementMode;

    // Vision variables
    private bool isAlert;
    private float suspicionTime;
    private float blockedVisionRange, drawnVisionRange, drawnVisionAngle;

    private float waitTime = -1;

    private Vector2 lastPos; // For backtracking

    private LayerMask raycastLayerMask;

    // References
    private Rigidbody2D myRigidbody, player1Rigidbody, player2Rigidbody;
    public Transform directionMarkerTransform;
    public GameObject visionConeObject, alertMarkerObject, alertSpriteMaskObject;
    private Mesh visionConeMesh;

    // Toggles
    public bool enableMove, betterVisionCone;


    void Start()
    {
        // Set references
        myRigidbody = GetComponent<Rigidbody2D>();
        player1Rigidbody = GameState.Instance.GetPlayerObject(Player.Player1).GetComponent<Rigidbody2D>();
        player2Rigidbody = GameState.Instance.GetPlayerObject(Player.Player2).GetComponent<Rigidbody2D>();

        // Set initial movement variables
        targetPosition = myRigidbody.position;
        targetAngle = myRigidbody.rotation;
        defaultRouteIndex = 0;
        queueIndex = 0;
        queue = new List<GuardRouteAction>();
        movementMode = MovementMode.Default;

        isAlert = false;

        drawnVisionRange = -1f;
        drawnVisionAngle = -1f;
        visionConeMesh = new Mesh();
        visionConeObject.GetComponent<MeshFilter>().mesh = visionConeMesh;

        raycastLayerMask = ~LayerMask.GetMask("Ignore Raycast", "Clickable");

        // Get initial movement target and vision collider
        if (defaultRouteActions.Count == 0)
        {
            defaultRouteActions = new List<GuardRouteAction>();
            defaultRouteActions.Add(GuardRouteAction.CreateGuardMoveAction(myRigidbody.position));
            defaultRouteActions.Add(GuardRouteAction.CreateGuardTurnAction(myRigidbody.rotation));
        }
        UpdateMoveTargets();
        CastVisionRay();
        DrawVisionCone();
        if (betterVisionCone) DrawBetterVisionCone();
    }

    void Update()
    {
        if (betterVisionCone) DrawBetterVisionCone();
        else
        {
            CastVisionRay();
            DrawVisionCone();
        }

        // Set alert marker position
        alertMarkerObject.transform.eulerAngles = new Vector3(0, 0, 0);
        alertMarkerObject.SetActive(isAlert);
        alertSpriteMaskObject.transform.localPosition = new Vector3(Mathf.Min(0, suspicionTime / secondsToCatch - 1), 0, 0);
    }

    void FixedUpdate()
    {
        if (waitTime >= 0) waitTime -= Time.fixedDeltaTime;
        bool updatedVision = UpdateVision(Time.fixedDeltaTime);
        if (!enableMove) return;

        float angleDifference = (targetAngle - myRigidbody.rotation) % 360;
        if (angleDifference > 180f) angleDifference -= 360;
        else if (angleDifference < -180f) angleDifference += 360;
        if (Mathf.Abs(angleDifference) > 180f) Debug.Log(angleDifference);
        if (Mathf.Abs(angleDifference) >= ANGLE_TOL)
        {
            // Rotate
            float rotationSpeed = Mathf.Min(Time.fixedDeltaTime * ROTATION_SPEED, Mathf.Abs(angleDifference));
            float rotationDelta = (angleDifference > 0) ? rotationSpeed : -rotationSpeed;
            myRigidbody.MoveRotation(myRigidbody.rotation + rotationDelta);
        }
        else if (Vector2.Distance(myRigidbody.position, targetPosition) >= POSITION_TOL)
        {
            if (!updatedVision || Vector2.Distance(myRigidbody.position, targetPosition) >= visionRange * targetChaseDistanceRatio)
            {
                // Move
                Vector2 movementDirection = targetPosition - myRigidbody.position;
                float theSpeed = isAlert ? chaseSpeed : moveSpeed;
                float moveDistance = Mathf.Min(Time.fixedDeltaTime * theSpeed / movementDirection.magnitude, 1f);
                myRigidbody.MovePosition(myRigidbody.position + movementDirection * moveDistance);
            }
        }
        else if (!updatedVision && waitTime < 0) UpdateMoveTargets();
    }

    // Updates the target position and angle of the guard based on its route
    private void UpdateMoveTargets()
    {
        if (queue.Count == 0 || (movementMode == MovementMode.Backtrack && queueIndex == 1))
        {
            // Set queue to next default waypoint if it is empty or finished
            queue = new List<GuardRouteAction>();
            queue.Add(defaultRouteActions[defaultRouteIndex]);
            defaultRouteIndex = (defaultRouteIndex + 1) % defaultRouteActions.Count;
            queueIndex = 0;
            movementMode = MovementMode.Default;
        }

        GuardRouteAction action;
        switch (movementMode)
        {
            case MovementMode.Default:
                action = queue[queueIndex];
                switch (action.actionType)
                {
                    case GuardRouteActionType.Move:
                        targetPosition = action.moveTarget;
                        targetAngle = Mathf.Atan2(targetPosition.y - myRigidbody.position.y, targetPosition.x - myRigidbody.position.x) * Mathf.Rad2Deg;
                        break;
                    case GuardRouteActionType.Turn:
                        targetAngle = action.turnTarget;
                        break;
                    case GuardRouteActionType.Wait:
                        waitTime = action.waitTime;
                        break;
                    default:
                        throw new System.Exception("Invalid GuardRouteActionType");
                }
                // lastPos = targetPosition;

                queueIndex++;
                if (queueIndex >= queue.Count)
                {
                    if (queue.Count == 1) movementMode = MovementMode.Backtrack;
                    else movementMode = MovementMode.LookLeft;
                }
                break;
            case MovementMode.LookLeft:
                targetAngle += 90;
                movementMode = MovementMode.LookRight;
                break;
            case MovementMode.LookRight:
                targetAngle -= 180;
                movementMode = MovementMode.Backtrack;
                break;
            case MovementMode.Backtrack:
                isAlert = false;
                queue.RemoveAt(queueIndex - 1);
                queueIndex--;
                action = queue[queueIndex - 1];
                switch (action.actionType)
                {
                    case GuardRouteActionType.Move:
                        targetPosition = action.moveTarget;
                        targetAngle = Mathf.Atan2(targetPosition.y - myRigidbody.position.y, targetPosition.x - myRigidbody.position.x) * Mathf.Rad2Deg;
                        break;
                    case GuardRouteActionType.Turn:
                        targetAngle = action.turnTarget;
                        break;
                    case GuardRouteActionType.Wait:
                        throw new System.Exception("Should never get to Wait on a Backtrack step");
                    default:
                        throw new System.Exception("Invalid GuardRouteActionType");
                }
                break;
        }
    }

    // Updates alert state based on players within vision, returning true if the movement target was updated
    private bool UpdateVision(float deltaTime)
    {
        // Look for players within vision range
        (bool player1Visible, float player1Distance) = CanSeePlayer(player1Rigidbody);
        (bool player2Visible, float player2Distance) = CanSeePlayer(player2Rigidbody);
        Player? foundPlayer = null;
        if (player1Visible && player2Visible)
        {
            foundPlayer = (player1Distance < player2Distance) ? Player.Player1 : Player.Player2;
        }
        else if (player1Visible) foundPlayer = Player.Player1;
        else if (player2Visible) foundPlayer = Player.Player2;

        if (foundPlayer.HasValue)
        {
            // Compute new chase waypoint
            Rigidbody2D playerRigidbody = (foundPlayer.Value == Player.Player1) ? player1Rigidbody : player2Rigidbody;
            targetPosition = playerRigidbody.position;
            targetAngle = Mathf.Atan2(targetPosition.y - myRigidbody.position.y, targetPosition.x - myRigidbody.position.x) * Mathf.Rad2Deg;

            GuardRouteAction newChaseWaypoint = new GuardRouteAction();
            newChaseWaypoint.moveTarget = targetPosition;

            if (isAlert)
            {
                // Handle suspicion
                suspicionTime += deltaTime;
                if (suspicionTime >= secondsToCatch) PlayerCaught(foundPlayer.Value);

                // Modify the current queue to chase
                if (movementMode == MovementMode.LookLeft) queue[queue.Count - 1] = newChaseWaypoint;
                else
                {
                    // movementMode is either LookRight or Backtrack
                    queue.Add(newChaseWaypoint);
                    queueIndex++;
                    movementMode = MovementMode.LookLeft;
                }
            }
            else
            {
                isAlert = true;
                suspicionTime = 0;
                waitTime = 0;

                // If currently "waiting", then replace Wait action with a position reset
                GuardRouteAction currentAction = queue[queue.Count - 1];
                if (currentAction.actionType == GuardRouteActionType.Wait)
                {
                    GuardRouteAction resetMoveAction = new GuardRouteAction();
                    resetMoveAction.moveTarget = myRigidbody.position;
                    queue[queue.Count - 1] = resetMoveAction;
                }
                // If currently "turning", then add a position reset action afterwards but skip it
                else if (currentAction.actionType == GuardRouteActionType.Turn)
                {
                    GuardRouteAction resetMoveAction = new GuardRouteAction();
                    resetMoveAction.moveTarget = myRigidbody.position;
                    queue.Add(resetMoveAction);
                    queueIndex++;
                }
                queue.Add(newChaseWaypoint);
                queueIndex++;
                movementMode = MovementMode.LookLeft;
            }
            return true;
        }

        return false;
    }

    private void PlayerCaught(Player caughtPlayer)
    {
        Debug.Log($"Player {(int)caughtPlayer + 1} was caught!");
    }

    private void CastVisionRay()
    {
        Vector2 direction = (Vector2)directionMarkerTransform.position - myRigidbody.position;
        RaycastHit2D raycast = Physics2D.Raycast(myRigidbody.position, direction, visionRange, raycastLayerMask);
        blockedVisionRange = (raycast.collider != null) ? raycast.distance : visionRange;
    }

    // Check if the guard can see a player at the given position. Returns (canSeePlayer, distanceToPlayer).
    private (bool, float) CanSeePlayer(Rigidbody2D playerRigidbody)
    {
        PolygonCollider2D visionConeCollider = visionConeObject.GetComponent<PolygonCollider2D>();
        if (playerRigidbody.IsTouching(visionConeCollider))
        {
            float angledDistance = (playerRigidbody.position - myRigidbody.position).magnitude;
            return (true, angledDistance);
        }
        return (false, 0);
    }

    private void DrawVisionCone()
    {
        if (Mathf.Abs(drawnVisionRange - blockedVisionRange) < 1e-5 && Mathf.Abs(drawnVisionAngle - visionAngle) < 1e-5) return;
        // Don't redraw unless necessary

        drawnVisionRange = blockedVisionRange;
        drawnVisionAngle = visionAngle;

        visionConeMesh.Clear();

        // Get vertices of mesh
        Vector2[] colliderPoints = new Vector2[visionConeResolution + 2];
        colliderPoints[0] = new Vector2(0, 0);
        float angle = -visionAngle;
        float arcLength = 2 * visionAngle;
        for (int i = 0; i <= visionConeResolution; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * blockedVisionRange;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * blockedVisionRange;

            colliderPoints[i + 1] = new Vector2(x, y);

            angle += (arcLength / visionConeResolution);
        }
        Vector3[] meshPoints = System.Array.ConvertAll<Vector2, Vector3>(colliderPoints, point => point);

        // Calculate triangles array
        int[] triangles = new int[visionConeResolution * 3];
        for (int i = 0; i < visionConeResolution; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i + 2;
        }

        // Set mesh
        visionConeMesh.vertices = meshPoints;
        visionConeMesh.uv = new Vector2[visionConeResolution + 2];
        visionConeMesh.triangles = triangles;
        visionConeMesh.RecalculateNormals();
        visionConeMesh.RecalculateBounds();

        // Set collider points
        PolygonCollider2D collider = visionConeObject.GetComponent<PolygonCollider2D>();
        collider.SetPath(0, colliderPoints);
    }

    private void DrawBetterVisionCone()
    {
        // Get vertices of mesh
        Vector2[] colliderPoints = new Vector2[visionConeResolution + 2];
        colliderPoints[0] = new Vector2(0, 0);
        float angle = -visionAngle;
        float arcLength = 2 * visionAngle;
        for (int i = 0; i <= visionConeResolution; i++)
        {
            float worldAngle = transform.rotation.eulerAngles.z + angle;
            Vector2 raycastDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * worldAngle), Mathf.Sin(Mathf.Deg2Rad * worldAngle));
            RaycastHit2D raycastHit = Physics2D.Raycast(myRigidbody.position, raycastDirection, visionRange, raycastLayerMask);
            float raycastRange = (raycastHit.collider != null) ? raycastHit.distance : visionRange;

            Vector2 vertexDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
            // Debug.Log($"Angle: {angle}, World Angle: {worldAngle}, Raycast direction: {raycastDirection}, Raycast range: {raycastRange}, v: {vertexDirection}");
            colliderPoints[i + 1] = vertexDirection * raycastRange;

            angle += (arcLength / visionConeResolution);
        }

        Vector3[] meshPoints = System.Array.ConvertAll<Vector2, Vector3>(colliderPoints, point => point);

        // Calculate triangles array
        int[] triangles = new int[visionConeResolution * 3];
        for (int i = 0; i < visionConeResolution; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i + 2;
        }

        // Set mesh
        visionConeMesh.Clear();
        visionConeMesh.vertices = meshPoints;
        visionConeMesh.uv = new Vector2[visionConeResolution + 2];
        visionConeMesh.triangles = triangles;
        visionConeMesh.RecalculateNormals();
        visionConeMesh.RecalculateBounds();

        // Set collider points
        PolygonCollider2D collider = visionConeObject.GetComponent<PolygonCollider2D>();
        collider.SetPath(0, colliderPoints);
    }
}


[System.Serializable]
public class GuardRouteAction
{
    public GuardRouteActionType actionType;

    public Vector2 moveTarget;
    public float turnTarget;
    public float waitTime;

    public GuardRouteAction()
    {
        actionType = GuardRouteActionType.Move;
        moveTarget = new Vector2(0, 0);
        turnTarget = 0;
        waitTime = 0;
    }

    public static GuardRouteAction CreateGuardMoveAction(Vector2 target)
    {
        GuardRouteAction action = new GuardRouteAction();
        action.moveTarget = target;
        return action;
    }

    public static GuardRouteAction CreateGuardTurnAction(float angle)
    {
        GuardRouteAction action = new GuardRouteAction();
        action.actionType = GuardRouteActionType.Turn;
        action.turnTarget = angle;
        return action;
    }

    public static GuardRouteAction CreateGuardWaitAction(float seconds)
    {
        GuardRouteAction action = new GuardRouteAction();
        action.actionType = GuardRouteActionType.Wait;
        action.waitTime = seconds;
        return action;
    }
}
