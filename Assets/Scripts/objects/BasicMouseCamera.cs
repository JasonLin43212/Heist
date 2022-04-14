using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BasicMouseCamera : MonoBehaviour
{
    // Constants
    const float ANGLE_TOL = 0.3f;

    // Modifiable constants
    public float visionRange = 7f, visionAngle = 10f;
    public float rotationSpeed = 10f;
    public float secondsToCatch = 0.2f;
    public float catchRateMultiplierMin, catchRateMultiplierMax;
    public int visionConeResolution = 50;

    // State variables
    private bool cameraEnabled;

    // Movement variables
    public List<float> defaultAngleRoute;
    private int routeIndex;
    private float targetAngle;

    // Vision variables
    private bool isAlert;
    private float suspicionTime;
    // Legacy vision system
    // private float blockedVisionRange, drawnVisionRange, drawnVisionAngle;

    // References
    private Rigidbody2D player1Rigidbody, player2Rigidbody;
    public MouseEventHandler mouseEventHandler;
    public Transform directionMarkerTransform;
    public GameObject visionConeObject, alertMarkerObject, alertSpriteMaskObject;
    private Mesh visionConeMesh;

    // Toggles
    public bool enableMove = false; // , betterVisionCone = true;


    void Start()
    {
        // Set references
        player1Rigidbody = GameState.Instance.GetPlayerObject(Player.Player1).GetComponent<Rigidbody2D>();
        player2Rigidbody = GameState.Instance.GetPlayerObject(Player.Player2).GetComponent<Rigidbody2D>();

        cameraEnabled = true;
        isAlert = false;

        // Legacy vision system
        // drawnVisionRange = -1f;
        // drawnVisionAngle = -1f;
        visionConeMesh = new Mesh();
        visionConeObject.GetComponent<MeshFilter>().mesh = visionConeMesh;

        // Get initial movement targets and vision collider
        if (defaultAngleRoute.Count == 0) enableMove = false;
        UpdateMoveTargets();
        DrawVisionCone();
    }

    void Update()
    {
        DrawVisionCone();

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

        // Check enabled/disabled
        HandleMouseInteraction();
        visionConeObject.SetActive(cameraEnabled);
    }

    // Set enabled/disabled based on if the mouse is holding down on the camera
    private void HandleMouseInteraction()
    {
        cameraEnabled = !mouseEventHandler.HasMouseDown();
        if (!cameraEnabled) isAlert = false;
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
        Vector2? player1Sighted = VisionUtils.CanSeePlayer(visionConeObject, player1Rigidbody);
        Vector2? player2Sighted = VisionUtils.CanSeePlayer(visionConeObject, player2Rigidbody);
        Player? foundPlayer = null;
        if (player1Sighted.HasValue && player2Sighted.HasValue)
        {
            float player1Distance = (player1Sighted.Value - (Vector2)transform.position).magnitude;
            float player2Distance = (player2Sighted.Value - (Vector2)transform.position).magnitude;
            foundPlayer = (player1Distance < player2Distance) ? Player.Player1 : Player.Player2;
        }
        else if (player1Sighted.HasValue) foundPlayer = Player.Player1;
        else if (player2Sighted.HasValue) foundPlayer = Player.Player2;

        if (foundPlayer.HasValue)
        {
            if (isAlert)
            {
                Vector2 playerPosition = (foundPlayer == Player.Player1) ? player1Sighted.Value : player2Sighted.Value;
                float playerDistance = (playerPosition - (Vector2)transform.position).magnitude;
                // The multiplier to deltaTime is scaled linearly based on distance between catchRateMultiplierMin and catchRateMultiplierMax
                float timeIncrementMultiplier = (1 - playerDistance / visionRange) * (catchRateMultiplierMax - catchRateMultiplierMin) + catchRateMultiplierMin;
                suspicionTime += deltaTime * timeIncrementMultiplier;  // Increment alert timer
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

    private void DrawVisionCone()
    {
        VisionUtils.DrawBetterVisionCone(
            originTransform: transform,
            visionConeMesh: visionConeMesh,
            visionConeObject: visionConeObject,
            visionConeResolution: visionConeResolution,
            visionAngle: visionAngle,
            visionRange: visionRange
        );
        VisionUtils.UpdateVisionConeColor(visionConeObject, suspicionTime, secondsToCatch);
    }

    private void PlayerCaught(Player caughtPlayer)
    {
        GameState.Instance.ControllerScript.PlayerCaught(caughtPlayer);
    }
}
