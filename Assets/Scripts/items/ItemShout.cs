using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShout : ItemDescriptor
{
    public override string Name => "Shout";

    public override bool CanUseWithKey => true;


    // Parameters
    [Min(0f)]
    public float guardDistractTime = 0.2f;
    [Min(0f)]
    public float noiseRange = 5f;

    public GameObject shoutImage;
    protected override float ItemUseRange => noiseRange;

    // Private variables
    private ContactFilter2D guardContactFilter;

    protected override void Start()
    {
        base.Start();
        showPlayerRangeCircle = false;
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

        // Creates a soundwave and sets its position
        GameObject currentShout = Instantiate(shoutImage);
        currentShout.GetComponent<Soundwave>().setPosition(playerPosition);
        // From the player position, it emits a shockwave that will hit all guards within range



        Collider2D playerRangeCollider = playerObject.GetComponent<PlayerMovement>().GetRangeCollider();
        List<Collider2D> colliderResults = new List<Collider2D>();
        int numOverlappingColliders = playerRangeCollider.OverlapCollider(guardContactFilter, colliderResults);

        if (numOverlappingColliders == 0) return false;  // no guards found within range

        foreach (Collider2D guardCollider in colliderResults)
        {
            GameObject guardObject = guardCollider.gameObject;
            guardObject.GetComponent<GuardBehaviour>().DistractGuard(playerPosition, guardDistractTime);
        }

        return true;
    }
}
