using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCrowbar : ItemDescriptor
{
    public override string Name => "Crowbar";

    public override bool CanUseWithKey => true;
    public AudioSource hitSound;

    // Parameters
    [Min(0f)]
    public float guardDisableTime = 5f;
    [Min(0f)]
    public float crowbarUseRange = 1f;
    protected override float ItemUseRange => crowbarUseRange;

    // Private variables
    private ContactFilter2D guardContactFilter;

    protected override void Start()
    {
        base.Start();
        guardContactFilter = new ContactFilter2D();
        guardContactFilter.SetLayerMask(LayerMask.GetMask("Guard"));
        // guardContactFilter.useTriggers = true;
    }

    public override bool UseKeyPressed()
    {
        // TODO: figure out when disabling the guard should work in terms of collisions
        // For now, crowbar has a range and we pick the guard closest to the player, if one exists
        Player? heldPlayer = itemBehaviour.GetHolder();
        if (!heldPlayer.HasValue) return false;  // item isn't actually being held?
        GameObject playerObject = GameState.Instance.GetPlayerObject(heldPlayer.Value);
        Vector2 playerPosition = (Vector2)playerObject.transform.position;

        Collider2D playerRangeCollider = playerObject.GetComponent<PlayerMovement>().GetRangeCollider();
        List<Collider2D> colliderResults = new List<Collider2D>();
        int numOverlappingColliders = playerRangeCollider.OverlapCollider(guardContactFilter, colliderResults);

        if (numOverlappingColliders == 0) return false;  // no guards found within range

        GameObject closestGuard = colliderResults[0].gameObject;
        float shortestDistance = float.PositiveInfinity;
        foreach (Collider2D guardCollider in colliderResults)
        {
            GameObject guardObject = guardCollider.gameObject;
            float distance = ((Vector2)guardObject.transform.position - playerPosition).magnitude;
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestGuard = guardObject;
            }
        }
        hitSound.Play();
        return closestGuard.GetComponent<GuardBehaviour>().DisableGuard(guardDisableTime);
    }
}
