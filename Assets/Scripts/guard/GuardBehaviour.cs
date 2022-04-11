using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBehaviour : MonoBehaviour
{
    // Constants
    const float MOVE_SPEED = 1.5f, ROTATION_SPEED = 180f, ANGLE_TOL = 1f, POSITION_TOL = 0.05f;  // movement
    const float VISION_RANGE = 3f, VISION_ANGLE = 15f;
    const float PLAYER_RADIUS = 0.5f;
    const float SUSPICION_THRESHOLD = 2f;

    const int DRAW_VISION_SEGMENTS = 40;

    // Movement variables
    public List<Vector2> defaultRoute;
    private int defaultRouteIndex, queueIndex;
    private List<Vector2> queue;
    private Vector2 targetPosition;
    private float targetAngle;

    // Vision variables
    public bool isAlert;
    public float suspicionTime;
    private float blockedVisionRange, drawnVisionRange;

    // References
    private Rigidbody2D myRigidbody;
    public Transform directionMarkerTransform;
    public GameObject visionConeObject, alertMarkerObject;
    private Mesh visionConeMesh;

    // TEMPORARY
    public Rigidbody2D player1Rigidbody, player2Rigidbody;

    // Toggles
    public bool enableMove;


    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        defaultRouteIndex = 0;
        queueIndex = 0;
        queue = new List<Vector2>();

        isAlert = false;

        drawnVisionRange = -1f;
        visionConeMesh = new Mesh();
        visionConeObject.GetComponent<MeshFilter>().mesh = visionConeMesh;

        UpdateMoveTargets();
        CastVisionRay();
        DrawVisionCone();
    }

    void Update()
    {
        CastVisionRay();
        DrawVisionCone();
        UpdateVision(Time.deltaTime);

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

    // Updates alert state based on players within vision
    private void UpdateVision(float deltaTime)
    {
        // Look for players within vision
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
            if (foundPlayer != null)
            {
                suspicionTime += deltaTime;  // Increment alert timer
                // TODO: update search sequence in movement queue
            }
            else
            {
                isAlert = false;
            }
        }
        else
        {
            if (foundPlayer != null)
            {
                isAlert = true;
                suspicionTime = 0;

                // TODO: add search sequence to movement queue
            }
        }

    }

    private void CastVisionRay()
    {
        Vector2 direction = (Vector2)directionMarkerTransform.position - myRigidbody.position;
        RaycastHit2D raycast = Physics2D.Raycast(myRigidbody.position, direction, VISION_RANGE);
        blockedVisionRange = (raycast.collider != null) ? raycast.distance : VISION_RANGE;
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
        if (Mathf.Abs(drawnVisionRange - blockedVisionRange) < 1e-5) return;  // Don't redraw unless necessary
        drawnVisionRange = blockedVisionRange;
        visionConeMesh.Clear();

        // Get vertices of mesh
        Vector2[] colliderPoints = new Vector2[DRAW_VISION_SEGMENTS + 2];
        colliderPoints[0] = new Vector2(0, 0);
        float angle = -VISION_ANGLE;
        float arcLength = 2 * VISION_ANGLE;
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
