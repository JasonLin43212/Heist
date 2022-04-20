using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BasicMouseCamera : MonoBehaviour
{
    // Constants
    const float ANGLE_TOL = 0.3f;

    // Modifiable constants
    public float visionRange = 7f, visionAngle = 10f;
    public float rotationSpeed = 10f;
    public float secondsToCatch = 0.2f;
    public int visionConeResolution = 50;
    public float timerLimit = 30f;
    public TMP_Text timerText;

    // State variables
    private bool cameraEnabled;
    private bool timerIsCountingDown = false;
    private float timerUntilEnabled;

    // Movement variables
    public List<float> defaultAngleRoute;
    private int routeIndex;
    private float targetAngle;

    // Vision variables
    private bool isAlert;
    private float suspicionTime;
    private float blockedVisionRange, drawnVisionRange, drawnVisionAngle;

    private LayerMask raycastLayerMask;

    // References
    private Rigidbody2D player1Rigidbody, player2Rigidbody;
    public MouseEventHandler mouseEventHandler;
    public Transform directionMarkerTransform, canvasTransform;
    public GameObject visionConeObject, alertMarkerObject, alertSpriteMaskObject;
    private Mesh visionConeMesh;

    // Toggles
    public bool enableMove = false, betterVisionCone = true;


    void Start()
    {
        // Set references
        player1Rigidbody = GameState.Instance.GetPlayerObject(Player.Player1).GetComponent<Rigidbody2D>();
        player2Rigidbody = GameState.Instance.GetPlayerObject(Player.Player2).GetComponent<Rigidbody2D>();

        cameraEnabled = true;
        isAlert = false;
        timerUntilEnabled = timerLimit;
        timerText.text = "";

        drawnVisionRange = -1f;
        drawnVisionAngle = -1f;
        visionConeMesh = new Mesh();
        visionConeObject.GetComponent<MeshFilter>().mesh = visionConeMesh;

        raycastLayerMask = ~LayerMask.GetMask("Ignore Raycast", "Clickable", "Guard");

        // Get initial movement targets and vision collider
        if (defaultAngleRoute.Count == 0) enableMove = false;
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

        bool updatedVision = UpdateVision(Time.deltaTime);
        if (!updatedVision && enableMove)
        {
            float angleDifference = (targetAngle - transform.eulerAngles.z) % 360;
            if (Mathf.Abs(angleDifference) > 180f) angleDifference = (360 - angleDifference) % 360;
            if (Mathf.Abs(angleDifference) >= ANGLE_TOL)
            {
                float cappedRotationSpeed = Mathf.Min(Time.fixedDeltaTime * rotationSpeed, Mathf.Abs(angleDifference));
                float rotationDelta = (angleDifference > 0) ? cappedRotationSpeed : -cappedRotationSpeed;
                transform.Rotate(0, 0, rotationDelta);
            }
            else UpdateMoveTargets();
        }

        // Set alert marker position
        alertMarkerObject.transform.eulerAngles = new Vector3(0, 0, 0);
        alertMarkerObject.SetActive(isAlert);
        alertSpriteMaskObject.transform.localPosition = new Vector3(Mathf.Min(0, suspicionTime / secondsToCatch - 1), 0, 0);

        canvasTransform.eulerAngles = new Vector3(0, 0, 0);

        // Check enabled/disabled
        HandleMouseInteraction();
        visionConeObject.SetActive(cameraEnabled);
        HandleTimerCountdown();
    }

    // Set enabled/disabled based on if the mouse is holding down on the camera
    private void HandleMouseInteraction()
    {
        bool disabled = mouseEventHandler.cameraIsDisabled();
        if (disabled && cameraEnabled)
        {
            cameraEnabled = false;
            timerIsCountingDown = true;
        }
        // cameraEnabled = !mouseEventHandler.HasMouseDown();
        if (!cameraEnabled) isAlert = false;
    }

    // Counts down timer and resets camera when done
    private void HandleTimerCountdown()
    {
        if (timerIsCountingDown)
        {
            timerText.text = Math.Truncate(timerUntilEnabled).ToString();
            timerUntilEnabled -= Time.deltaTime;
            if (timerUntilEnabled <= 0)
            {
                timerUntilEnabled = timerLimit;
                timerIsCountingDown = false;
                mouseEventHandler.enableCamera();
                cameraEnabled = true;
                timerText.text = "";
            }
        }
    }

    // Updates the target angle of the camera based on its route
    private void UpdateMoveTargets()
    {
        if (enableMove)
        {
            targetAngle = defaultAngleRoute[routeIndex];
            routeIndex = (routeIndex + 1) % defaultAngleRoute.Count;
        }
        else targetAngle = transform.eulerAngles.z;
    }

    // Updates alert state based on players within vision, returning true if a player is seen.
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
            if (isAlert)
            {
                suspicionTime += deltaTime;  // Increment alert timer
                // Catch player if has been suspicious for sufficient time
                if (suspicionTime >= secondsToCatch) PlayerCaught(foundPlayer.Value);
            }
            else
            {
                isAlert = true;
                suspicionTime = 0;
            }
            return true;
        }
        if (isAlert) isAlert = false;
        return false;
    }

    private void PlayerCaught(Player caughtPlayer)
    {
        GameState.Instance.ControllerScript.PlayerCaught(caughtPlayer);
    }

    private void CastVisionRay()
    {
        Vector2 myPosition = (Vector2)transform.position;
        Vector2 direction = (Vector2)directionMarkerTransform.position - myPosition;
        RaycastHit2D raycast = Physics2D.Raycast(myPosition, direction, visionRange, raycastLayerMask);
        blockedVisionRange = (raycast.collider != null) ? raycast.distance : visionRange;
    }

    // Check if the camera can see a player at the given position. Returns (canSeePlayer, distanceToPlayer).
    private (bool, float) CanSeePlayer(Rigidbody2D playerRigidbody)
    {
        PolygonCollider2D visionConeCollider = visionConeObject.GetComponent<PolygonCollider2D>();
        if (playerRigidbody.IsTouching(visionConeCollider))
        {
            Vector2 myPosition = (Vector2)transform.position;
            float angledDistance = (playerRigidbody.position - myPosition).magnitude;
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
            RaycastHit2D raycastHit = Physics2D.Raycast((Vector2)transform.position, raycastDirection, visionRange, raycastLayerMask);
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
