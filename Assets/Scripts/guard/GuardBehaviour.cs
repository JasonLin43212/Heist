using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GuardBehaviour : MonoBehaviour
{
    // Constants
    public float ROTATION_SPEED = 180f;
    const float ANGLE_TOL = 0.3f, POSITION_TOL = 0.05f;  // movement
    const float PLAYER_RADIUS = 0.4f;

    // Modifiable constants

    public float visionRange = 15f, visionAngle = 30f;
    public float targetChaseDistanceRatio = 0.7f;  // When pursuing a player, the guard tries to stay within visionRange * targetChaseDistanceRatio
    public float moveSpeed = 1.5f;

    public float chaseSpeed = 3f;
    public float secondsToCatch = 1.5f;
    [Min(0.1f)]
    public float catchRateMultiplierMin, catchRateMultiplierMax;
    public float suspicionDecreaseRate = 1f;
    public float disabledTimeMultiplier = 1f;  // multiply intended disable time by this amount (controls how hardy each guard is)
    public int visionConeResolution = 100;
    public TMP_Text timerText;
    public Transform canvasTransform;

    // Movement variables
    public List<GuardRouteAction> defaultRouteActions;
    private int defaultRouteIndex, queueIndex;
    private List<GuardRouteAction> queue;
    public Vector2 targetPosition;
    private float targetAngle;

    private enum MovementMode { Default, LookLeft, LookRight, Backtrack };
    private MovementMode movementMode;

    // Disabled
    private float disabledTime;
    private float distractedTime;
    private Color baseColor, disabledColor;

    // Vision variables
    private bool isAlert;
    private float suspicionTime;
    // Legacy vision system
    // private float blockedVisionRange, drawnVisionRange, drawnVisionAngle;

    private float waitTime = -1;

    // References
    private Rigidbody2D myRigidbody, player1Rigidbody, player2Rigidbody;
    public Transform directionMarkerTransform;
    public GameObject visionConeObject, alertMarkerObject, alertSpriteMaskObject;
    private Mesh visionConeMesh;

    // Toggles
    public bool enableMove, strictChasing;  // , betterVisionCone;

    // Debugging
    public string debugText;
    public bool enableDebugLogging = true;

    private string uniqueIdentifier;
    public string UniqueID => uniqueIdentifier;


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

        disabledTime = 0;
        distractedTime = 0;
        baseColor = GetComponent<SpriteRenderer>().material.color;
        disabledColor = new Color(baseColor.r - 0.3f, baseColor.g - 0.3f, baseColor.b - 0.3f, baseColor.a);

        isAlert = false;

        timerText.text = "";

        uniqueIdentifier = $"Guard<{myRigidbody.position.ToString()},{myRigidbody.rotation},{targetPosition.ToString()},{targetAngle}>";

        // Legacy vision system
        // drawnVisionRange = -1f;
        // drawnVisionAngle = -1f;
        visionConeMesh = new Mesh();
        visionConeObject.GetComponent<MeshFilter>().mesh = visionConeMesh;
        visionConeObject.tag = "GuardVisionCone";

        // Get initial movement target and vision collider
        if (defaultRouteActions.Count == 0)
        {
            defaultRouteActions = new List<GuardRouteAction>();
            defaultRouteActions.Add(GuardRouteAction.CreateGuardMoveAction(myRigidbody.position));
            defaultRouteActions.Add(GuardRouteAction.CreateGuardTurnAction(myRigidbody.rotation));
        }
        UpdateMoveTargets();
        DrawVisionCone();
    }

    void Update()
    {
        DrawVisionCone();

        // Set alert marker position
        alertMarkerObject.transform.eulerAngles = new Vector3(0, 0, 0);
        alertMarkerObject.SetActive(isAlert);
        alertSpriteMaskObject.transform.localPosition = new Vector3(Mathf.Min(0, suspicionTime / secondsToCatch - 1), 0, 0);
        canvasTransform.eulerAngles = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {
        // Handle guard being disabled
        if (disabledTime > 0)
        {
            timerText.text = Math.Truncate(disabledTime).ToString();
            disabledTime -= Time.fixedDeltaTime;
            if (disabledTime <= 0)
            {
                GetComponent<SpriteRenderer>().material.color = baseColor;
                timerText.text = "";
            }
            else return;
        }

        if (waitTime >= 0) waitTime -= Time.fixedDeltaTime;
        bool updatedVision = UpdateVision(Time.fixedDeltaTime);
        if (!enableMove) return;
        if (!strictChasing && updatedVision) return;

        float angleDifference = (targetAngle - myRigidbody.rotation) % 360;
        if (angleDifference > 180f) angleDifference -= 360;
        else if (angleDifference < -180f) angleDifference += 360;
        if (Mathf.Abs(angleDifference) > 180f) Debug.Log(angleDifference);
        if (Mathf.Abs(angleDifference) >= ANGLE_TOL || distractedTime > 0)
        {
            // Rotate
            if(distractedTime > 0)
                ROTATION_SPEED *= 3;
            float rotationSpeed = Mathf.Min(Time.fixedDeltaTime * ROTATION_SPEED, Mathf.Abs(angleDifference));
            float rotationDelta = (angleDifference > 0) ? rotationSpeed : -rotationSpeed;
            myRigidbody.MoveRotation(myRigidbody.rotation + rotationDelta);
            if(distractedTime > 0)
                ROTATION_SPEED /= 3;
            if(Mathf.Abs(angleDifference) < ANGLE_TOL)
            {
                distractedTime -= Time.fixedDeltaTime;
            }
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
                        if (enableDebugLogging) Debug.Log($"Default Move updated: {action}.");
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
                if (enableDebugLogging) Debug.Log($"Default movement updated: {action}. New movement mode: {movementMode}");
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
                if (enableDebugLogging) Debug.Log($"Backtracking (queue index: {queueIndex}). {QueueToString()}");
                queue.RemoveAt(queueIndex - 1);
                queueIndex--;
                action = queue[queueIndex - 1];
                if (enableDebugLogging) Debug.Log($"Backtracking going to {action}");
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
        Vector2? player1Sighted = VisionUtils.CanSeePlayer(visionConeObject, player1Rigidbody);
        Vector2? player2Sighted = VisionUtils.CanSeePlayer(visionConeObject, player2Rigidbody);
        Player? foundPlayer = null;
        if (player1Sighted.HasValue && player2Sighted.HasValue)
        {
            float player1Distance = (player1Sighted.Value - myRigidbody.position).magnitude;
            float player2Distance = (player2Sighted.Value - myRigidbody.position).magnitude;
            foundPlayer = (player1Distance < player2Distance) ? Player.Player1 : Player.Player2;
        }
        else if (player1Sighted.HasValue) foundPlayer = Player.Player1;
        else if (player2Sighted.HasValue) foundPlayer = Player.Player2;

        if (foundPlayer.HasValue)
        {
            // Compute new chase waypoint
            targetPosition = (foundPlayer.Value == Player.Player1) ? player1Sighted.Value : player2Sighted.Value;
            targetAngle = Mathf.Atan2(targetPosition.y - myRigidbody.position.y, targetPosition.x - myRigidbody.position.x) * Mathf.Rad2Deg;

            GuardRouteAction newChaseWaypoint = new GuardRouteAction();
            newChaseWaypoint.moveTarget = targetPosition;

            if (isAlert)
            {
                // Handle suspicion
                float playerDistance = (targetPosition - (Vector2)myRigidbody.position).magnitude;
                // The multiplier to deltaTime is scaled linearly based on distance between catchRateMultiplierMin and catchRateMultiplierMax
                float timeIncrementMultiplier = (1 - playerDistance / visionRange) * (catchRateMultiplierMax - catchRateMultiplierMin) + catchRateMultiplierMin;
                suspicionTime += deltaTime * timeIncrementMultiplier;
                if (suspicionTime >= secondsToCatch) PlayerCaught(foundPlayer.Value);

                // Modify the current queue to chase
                if (movementMode == MovementMode.LookLeft)
                {
                    queue[queue.Count - 1] = newChaseWaypoint;
                }
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
                    queue.RemoveAt(queue.Count - 1);
                    queueIndex--;
                }
                // Add a position reset action but skip it

                GuardRouteAction resetMoveAction = new GuardRouteAction();
                resetMoveAction.moveTarget = myRigidbody.position;
                queue.Add(resetMoveAction);
                queue.Add(newChaseWaypoint);
                queueIndex += 2;
                movementMode = MovementMode.LookLeft;
            }
            return true;
        }
        else
        {
            // Slowly return to yellow vision cone
            if (!isAlert && suspicionTime > 0)
            {
                suspicionTime = Mathf.Max(0f, suspicionTime - deltaTime * suspicionDecreaseRate);
            }
        }

        return false;
    }

    private void PlayerCaught(Player caughtPlayer)
    {
        GameState.Instance.GameController.PlayerCaught(caughtPlayer);
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

    public bool DisableGuard(float timeToDisableFor)
    {
        if (disabledTime > 0) return false;  // already disabled
        disabledTime = timeToDisableFor * disabledTimeMultiplier;
        GetComponent<SpriteRenderer>().material.color = disabledColor;
        return true;
    }

    public bool DistractGuard(Vector2 placeToLookAt, float timeToDistractFor) // The guard turns to look at the place
    {
        // print out a debug statement
        Debug.Log($"Distracting guard with time of {timeToDistractFor}");
        distractedTime = timeToDistractFor;
        suspicionTime += distractedTime;
        targetAngle = Mathf.Atan2(placeToLookAt.y - myRigidbody.position.y, placeToLookAt.x - myRigidbody.position.x) * Mathf.Rad2Deg;
        return true;
    }

    private string QueueToString()
    {
        string output = $"Queue<{queue.Count}>: [";
        foreach (GuardRouteAction queueAction in queue)
        {
            output = output + queueAction.ToString() + ", ";
        }
        output = output + "]";
        return output;
    }

    public GuardState Serialize()
    {
        return new GuardState(
            myRigidbody.position,
            myRigidbody.rotation,
            defaultRouteIndex,
            queueIndex,
            queue,
            targetPosition,
            targetAngle,
            (int)movementMode,
            disabledTime,
            distractedTime,
            isAlert,
            suspicionTime,
            waitTime
        );
    }

    public void Deserialize(GuardState state)
    {
        transform.position = new Vector3(state.position.x, state.position.y, 0f);
        transform.eulerAngles = new Vector3(0, 0, state.rotation);
        this.defaultRouteIndex = state.defaultRouteIndex;
        this.queueIndex = state.queueIndex;
        this.queue = state.queue;
        this.targetPosition = state.targetPosition;
        this.targetAngle = state.targetAngle;
        this.movementMode = (MovementMode)state.movementMode;
        this.disabledTime = state.disabledTime;
        this.distractedTime = state.distractedTime;
        this.isAlert = state.isAlert;
        this.suspicionTime = state.suspicionTime;
        this.waitTime = state.waitTime;
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

    public override string ToString()
    {
        switch (actionType)
        {
            case GuardRouteActionType.Move:
                return $"Move<{moveTarget}>";
            case GuardRouteActionType.Turn:
                return $"Turn<{turnTarget}>";
            case GuardRouteActionType.Wait:
                return $"Wait<{waitTime}>";
            default:
                return "EmptyAction";
        }
    }
}


[System.Serializable]
public struct GuardState
{
    public Vector2 position;
    public float rotation;

    public int defaultRouteIndex, queueIndex;
    public List<GuardRouteAction> queue;
    public Vector2 targetPosition;
    public float targetAngle;
    public int movementMode;
    public float disabledTime;

    public float distractedTime;
    public bool isAlert;
    public float suspicionTime;
    public float waitTime;

    public GuardState(
        Vector2 position,
        float rotation,
        int defaultRouteIndex,
        int queueIndex,
        List<GuardRouteAction> queue,
        Vector2 targetPosition,
        float targetAngle,
        int movementMode,
        float disabledTime,
        float distractedTime,
        bool isAlert,
        float suspicionTime,
        float waitTime
    )
    {
        this.position = position;
        this.rotation = rotation;
        this.defaultRouteIndex = defaultRouteIndex;
        this.queueIndex = queueIndex;
        this.queue = queue;
        this.targetPosition = targetPosition;
        this.targetAngle = targetAngle;
        this.movementMode = movementMode;
        this.disabledTime = disabledTime;
        this.distractedTime = distractedTime;
        this.isAlert = isAlert;
        this.suspicionTime = suspicionTime;
        this.waitTime = waitTime;
    }
}