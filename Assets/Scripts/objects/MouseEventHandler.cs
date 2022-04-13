using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEventHandler : MonoBehaviour
{
    private bool mouseDown = false;

    void OnMouseDown()
    {
        mouseDown = true;
    }

    void OnMouseUp()
    {
        mouseDown = false;
    }

    public bool HasMouseDown()
    {
        return mouseDown;
    }
}
