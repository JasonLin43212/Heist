using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDoorScript : DoorScript
{
    public ButtonScript button1, button2;
    public bool togglesPermanently = false;  // If set to true, after toggling for the first time,
                                             //the door cannot be reset to its previous state

    private bool wereButtonsPressed = false, doorLock = false;

    // Update is called once per frame
    protected override void Update()
    {
        if (doorLock) return;  // door is locked, cannot open again

        bool bothButtonsPressed = button1.IsTouchingPlayer && button2.IsTouchingPlayer;
        if (!wereButtonsPressed && bothButtonsPressed)
        {
            closed = !closed;
            doorLock = togglesPermanently;
        }
        wereButtonsPressed = bothButtonsPressed;

        base.Update();
    }
}
