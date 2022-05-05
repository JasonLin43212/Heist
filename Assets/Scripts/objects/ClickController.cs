using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickController : MonoBehaviour
{
    private ContactFilter2D colliderContactFilter;

    private GameObject targetObject = null;

    // Start is called before the first frame update
    void Start()
    {
        colliderContactFilter = new ContactFilter2D();
        colliderContactFilter.SetLayerMask(LayerMask.GetMask("Clickable"));
        colliderContactFilter.useTriggers = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.isGamePaused && !PauseMenu.PausedByCutscene && !PauseMenu.popupIsEnabled)
        {
            UpdateTargets(GetMouseWorldPosition());
            if (Input.GetMouseButtonDown(0)) HandleClick();
        }
        else
        {
            targetObject = null;
        }
    }

    private void HandleClick()
    {
        if (targetObject == null) return;  // Not over object

        // Try camera
        BasicMouseCamera cameraScript = targetObject.GetComponent<BasicMouseCamera>();
        if (cameraScript != null)
        {
            cameraScript.OnClick();
            return;
        }

        // Try door
        ClickDoorBehavior doorScript = targetObject.GetComponent<ClickDoorBehavior>();
        if (doorScript != null)
        {
            doorScript.OnClick();
            return;
        }
    }

    private void UpdateTargets(Vector2 mousePos)
    {
        // Update private variables to the current mouse-camera/door target if they exist
        List<Collider2D> overlappingColliders = GetCollidersMouseOver(mousePos);
        GameObject bestObject = null;
        float shortestDistance = float.PositiveInfinity;

        foreach (Collider2D candidateCollider in overlappingColliders)
        {
            float distance = ((Vector2)candidateCollider.transform.position - mousePos).magnitude;
            GameObject candidateGameObject = candidateCollider.transform.parent.gameObject;
            if (distance < shortestDistance)
            {
                bestObject = candidateGameObject;
                shortestDistance = distance;
            }
        }

        targetObject = bestObject;
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        Player cameraPlayer = (mousePos.x > Screen.width / 2) ? Player.Player2 : Player.Player1;
        Camera correctCamera = GameState.Instance.GetPlayerCamera(cameraPlayer);
        Vector2 worldPos = (Vector2)correctCamera.ScreenToWorldPoint(mousePos);
        return worldPos;
    }

    private List<Collider2D> GetCollidersMouseOver(Vector2 mousePos)
    {
        List<Collider2D> overlappingColliders = new List<Collider2D>();
        Physics2D.OverlapPoint(mousePos, colliderContactFilter, overlappingColliders);
        return overlappingColliders;
    }

    public bool IsTargetObject(GameObject candidate) => GameObject.ReferenceEquals(targetObject, candidate);

}
