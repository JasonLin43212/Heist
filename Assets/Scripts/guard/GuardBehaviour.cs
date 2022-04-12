using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBehaviour : MonoBehaviour
{
    // Constants
    const float MOVE_SPEED = 1.5f, ROTATION_SPEED = 120f, ANGLE_TOL = 0.3f, POSITION_TOL = 0.05f;  // movement
    const float PLAYER_RADIUS = 0.5f;
    const int DRAW_VISION_SEGMENTS = 40;

    // Modifiable constants

    public float visionRange = 5f, visionAngle = 10f;
    public float secondsToCatch = 2f;

    // Movement variables
    public List<Vector2> defaultRoute;
    private int defaultRouteIndex, queueIndex;
    private List<Vector2> queue;
    private Vector2 targetPosition;
    private float targetAngle;

    private enum MovementMode { Default, LookLeft, LookRight, Backtrack };
    private MovementMode movementMode;

    // Vision variables
    private bool isAlert;
    private float suspicionTime;
    private float blockedVisionRange, drawnVisionRange, drawnVisionAngle;

    // References
    private Rigidbody2D myRigidbody, player1Rigidbody, player2Rigidbody;
    public Transform directionMarkerTransform;
    public GameObject visionConeObject, alertMarkerObject;
    private Mesh visionConeMesh;

    // Toggles
    public bool enableMove;


    void Start()
    {
        // Set references
        myRigidbody = GetComponent<Rigidbody2D>();
        player1Rigidbody = GameState.Instance.GetPlayerObject(Player.Player1).GetComponent<Rigidbody2D>();
        player2Rigidbody = GameState.Instance.GetPlayerObject(Player.Player2).GetComponent<Rigidbody2D>();

        // Set initial movement variables
        defaultRouteIndex = 0;
        queueIndex = 0;
        queue = new List<Vector2>();
        movementMode = MovementMode.Default;

        isAlert = false;

        drawnVisionRange = -1f;
        drawnVisionAngle = -1f;
        visionConeMesh = new Mesh();
        visionConeObject.GetComponent<MeshFilter>().mesh = visionConeMesh;

        // Get initial movement target and vision collider
        UpdateMoveTargets();
        CastVisionRay();
        DrawVisionCone();
    }

    void Update()
    {
        CastVisionRay();
        DrawVisionCone();

        // Set alert marker position
        alertMarkerObject.transform.eulerAngles = new Vector3(0, 0, 0);
        alertMarkerObject.SetActive(isAlert);
    }

    void FixedUpdate()
    {
        if (enableMove)
        {
            float angleDifference = targetAngle % 360 - myRigidbody.rotation % 360;
            if (Mathf.Abs(angleDifference) >= ANGLE_TOL)
            {
                // Rotate
                float rotationSpeed = Mathf.Min(Time.fixedDeltaTime * ROTATION_SPEED, Mathf.Abs(angleDifference));
                float rotationDelta = (angleDifference > 0) ? rotationSpeed : -rotationSpeed;
                myRigidbody.MoveRotation(myRigidbody.rotation + rotationDelta);
            }
            else if (Vector2.Distance(myRigidbody.position, targetPosition) >= POSITION_TOL)
            {
                // Move
                if (!UpdateVision(Time.fixedDeltaTime))
                {
                    Vector2 movementDirection = targetPosition - myRigidbody.position;
                    float moveDistance = Mathf.Min(Time.fixedDeltaTime * MOVE_SPEED / movementDirection.magnitude, 1f);
                    myRigidbody.MovePosition(myRigidbody.position + movementDirection * moveDistance);
                }
            }
            else
            {
                if (!UpdateVision(Time.fixedDeltaTime)) UpdateMoveTargets();
            }
        }
    }

    // Updates the target position and angle of the guard based on its route
    private void UpdateMoveTargets()
    {
        if (queue.Count == 0 || (movementMode == MovementMode.Backtrack && queueIndex == 1))
        {
            // Set queue to next default waypoint if it is empty or finished
            queue = new List<Vector2>();
            queue.Add(defaultRoute[defaultRouteIndex]);
            defaultRouteIndex = (defaultRouteIndex + 1) % defaultRoute.Count;
            queueIndex = 0;
            movementMode = MovementMode.Default;
        }

        switch (movementMode)
        {
            case MovementMode.Default:
                targetPosition = queue[queueIndex];
                targetAngle = Mathf.Atan2(targetPosition.y - myRigidbody.position.y, targetPosition.x - myRigidbody.position.x) * Mathf.Rad2Deg;
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
                // TODO: update queue so that interrupted sightings are handled properly
                queue.RemoveAt(queueIndex - 1);
                queueIndex--;
                targetPosition = queue[queueIndex - 1];
                targetAngle = Mathf.Atan2(targetPosition.y - myRigidbody.position.y, targetPosition.x - myRigidbody.position.x) * Mathf.Rad2Deg;
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

        if (isAlert)
        {
            if (foundPlayer.HasValue)
            {
                suspicionTime += deltaTime;  // Increment alert timer

                // Catch player if has been suspicious for sufficient time
                if (suspicionTime >= secondsToCatch) PlayerCaught(foundPlayer.Value);

                // The chase continues: update waypoint in movement queue
                float distance = (foundPlayer.Value == Player.Player1) ? player1Distance : player2Distance;
                Vector2 movementDirection = (Vector2)directionMarkerTransform.position - myRigidbody.position;
                targetPosition = myRigidbody.position + movementDirection / movementDirection.magnitude * distance;
                if (movementMode == MovementMode.LookLeft) queue[queue.Count - 1] = targetPosition;
                else
                {
                    // movementMode is either LookRight or Backtrack
                    queue.Add(targetPosition);
                    queueIndex++;
                    movementMode = MovementMode.LookLeft;
                }
                return true;
            }
        }
        else
        {
            if (foundPlayer.HasValue)
            {
                isAlert = true;
                suspicionTime = 0;

                // Add new waypoint to movement queue to chase down player
                float distance = (foundPlayer.Value == Player.Player1) ? player1Distance : player2Distance;
                Vector2 movementDirection = (Vector2)directionMarkerTransform.position - myRigidbody.position;
                targetPosition = myRigidbody.position + movementDirection / movementDirection.magnitude * distance;
                queue.Add(targetPosition);
                queueIndex++;
                movementMode = MovementMode.LookLeft;
                return true;
            }
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
        RaycastHit2D raycast = Physics2D.Raycast(myRigidbody.position, direction, visionRange);
        blockedVisionRange = (raycast.collider != null) ? raycast.distance : visionRange;
    }

    // Check if the guard can see a player at the given position. Returns (canSeePlayer, distanceToPlayer).
    private (bool, float) CanSeePlayer(Rigidbody2D playerRigidbody)
    {
        PolygonCollider2D visionConeCollider = visionConeObject.GetComponent<PolygonCollider2D>();
        if (playerRigidbody.IsTouching(visionConeCollider)) return (true, (playerRigidbody.position - myRigidbody.position).magnitude);
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
        Vector2[] colliderPoints = new Vector2[DRAW_VISION_SEGMENTS + 2];
        colliderPoints[0] = new Vector2(0, 0);
        float angle = -visionAngle;
        float arcLength = 2 * visionAngle;
        for (int i = 0; i <= DRAW_VISION_SEGMENTS; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * blockedVisionRange;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * blockedVisionRange;

            colliderPoints[i + 1] = new Vector2(x, y);

            angle += (arcLength / DRAW_VISION_SEGMENTS);
        }
        Vector3[] meshPoints = System.Array.ConvertAll<Vector2, Vector3>(colliderPoints, point => point);

        // Calculate triangles array
        int[] triangles = new int[DRAW_VISION_SEGMENTS * 3];
        for (int i = 0; i < DRAW_VISION_SEGMENTS; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i + 2;
        }

        // Set mesh
        visionConeMesh.vertices = meshPoints;
        visionConeMesh.uv = new Vector2[DRAW_VISION_SEGMENTS + 2];
        visionConeMesh.triangles = triangles;
        visionConeMesh.RecalculateNormals();
        visionConeMesh.RecalculateBounds();

        // Set collider points
        PolygonCollider2D collider = visionConeObject.GetComponent<PolygonCollider2D>();
        collider.enabled = false;
        collider.SetPath(0, colliderPoints);
        collider.enabled = true;
    }

}
