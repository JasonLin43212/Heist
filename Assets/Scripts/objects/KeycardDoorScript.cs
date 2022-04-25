using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardDoorScript : DoorScript
{
    public GameObject spriteOutlineObject;

    public void Highlight(bool highlightOn)
    {
        spriteOutlineObject.SetActive(highlightOn);
    }

    public void Toggle()
    {
        if (closed)
        {
            closed = false;
        }
        else
        {
            closed = true;
        }
    }
}
