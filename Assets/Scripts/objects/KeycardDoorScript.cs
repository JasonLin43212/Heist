using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardDoorScript : DoorScript
{
    public GameObject spriteOutlineObject;

    public bool toggleOnceOnly = false;

    private bool doorLock = false;

    public void Highlight(bool highlightOn)
    {
        spriteOutlineObject.SetActive(highlightOn && !doorLock);
    }

    public void Toggle()
    {
        if (doorLock) return;
        if (closed)
        {
            closed = false;
        }
        else
        {
            closed = true;
        }
        if (toggleOnceOnly)
        {
            doorLock = true;
            Highlight(false);
        }
    }
}
