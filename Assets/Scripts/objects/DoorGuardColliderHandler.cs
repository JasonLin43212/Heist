using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGuardColliderHandler : MonoBehaviour
{
    public DoorScript parentScript;

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "GuardVisionCone" || collider.gameObject.tag == "GuardCollisionBox")
        {
            parentScript.SetGuardNotLooking(false);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "GuardVisionCone" || collider.gameObject.tag == "GuardCollisionBox")
        {
            parentScript.SetGuardNotLooking(true);
        }
    }
}
