using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardDoorScript : DoorScript
{
    public GameObject spriteOutlineObject;

    public bool toggleOnceOnly = false;

    public AudioSource openSound;
    public AudioSource closeSound;
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
            openSound.Play();
        }
        else
        {
            closed = true;
            closeSound.Play();
        }
        if (toggleOnceOnly)
        {
            doorLock = true;
            Highlight(false);
        }
    }

    public bool getDoorLock() => doorLock;
    protected override string SerializeDoor() => doorLock.ToString();
    protected override void DeserializeDoor(string state)
    {
        doorLock = state == "true";
    }
}
