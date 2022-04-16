using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEventHandler : MonoBehaviour
{
    private bool mouseDown = false;
    private static int numberOfCamerasDisabled = 0;
    private bool isDisabled = false;


    void OnMouseDown()
    {
        if (numberOfCamerasDisabled < 2){
            numberOfCamerasDisabled += 1;
            isDisabled = true;
        }
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

    public bool cameraIsDisabled(){
        return isDisabled;
    }

    public void enableCamera(){
        isDisabled = false;
        numberOfCamerasDisabled -=1;
    }
}
