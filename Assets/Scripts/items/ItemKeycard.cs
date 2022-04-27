using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemKeycard : ItemDescriptor
{
    public override string Name => "Keycard";

    public override bool CanUseWithKey => true;

    // Parameters
    public List<GameObject> interactableObjects;
    [Min(0f)]
    private float keycardUseRange = 1.5f;
    protected override float ItemUseRange => keycardUseRange;

    // Private variables
    private ContactFilter2D contactFilter;
    private GameObject currentTargetObject;

    protected override void Start()
    {
        base.Start();
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("KeycardObject"));
        // contactFilter.useTriggers = true;
    }

    protected override void Update()
    {
        base.Update();
        // Get the player's position
        Player? heldPlayer = itemBehaviour.GetHolder();
        if (!heldPlayer.HasValue)
        {
            // item isn't actually being held
            SetHighlight(currentTargetObject, false);
            currentTargetObject = null;
            return;
        }
        GameObject playerObject = GameState.Instance.GetPlayerObject(heldPlayer.Value);
        Vector2 playerPosition = (Vector2)playerObject.transform.position;

        Collider2D playerRangeCollider = playerObject.GetComponent<PlayerMovement>().GetRangeCollider();
        List<Collider2D> colliderResults = new List<Collider2D>();
        int numOverlappingColliders = playerRangeCollider.OverlapCollider(contactFilter, colliderResults);

        GameObject closestTarget = null;
        float shortestDistance = float.PositiveInfinity;
        foreach (Collider2D targetCollider in colliderResults)
        {
            GameObject targetObject = targetCollider.gameObject;
            if (!interactableObjects.Contains(targetObject))
            {
                targetObject = targetObject.transform.parent.gameObject;
                if (!interactableObjects.Contains(targetObject)) continue;
            }
            float distance = ((Vector2)targetObject.transform.position - playerPosition).magnitude;
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestTarget = targetObject;
            }
        }

        if (currentTargetObject != closestTarget)
        {
            SetHighlight(currentTargetObject, false);
            currentTargetObject = closestTarget;
            SetHighlight(currentTargetObject, true);
        }
    }

    public override bool UseKeyPressed()
    {
        if (currentTargetObject == null) return false;
        KeycardDoorScript keycardDoor = currentTargetObject.GetComponent<KeycardDoorScript>();
        if (keycardDoor != null)
        {
            keycardDoor.Toggle();
            return true;
        }

        return false;
    }

    private void SetHighlight(GameObject target, bool highlightOn)
    {
        if (target != null)
        {
            KeycardDoorScript keycardDoor = target.GetComponent<KeycardDoorScript>();
            if (keycardDoor != null) keycardDoor.Highlight(highlightOn);
        }
    }
}
