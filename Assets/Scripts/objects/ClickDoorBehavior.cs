using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDoorBehavior : DoorScript
{
    public GameObject spriteOutlineObject;

    protected override void Update()
    {
        base.Update();

        spriteOutlineObject.SetActive(GameState.Instance.ClickController.IsTargetObject(gameObject));
    }
    public void OnClick()
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
