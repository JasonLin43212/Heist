using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEventHandler : MonoBehaviour
{
    private bool mouseDown = false;
    private static int numberOfCamerasDisabled = 0;
    private bool isDisabled = false;
    public bool canBeReset = false;
    public bool resetTimer = false;


    void OnMouseDown()
    {
        if (!isDisabled && numberOfCamerasDisabled < 2){
            numberOfCamerasDisabled += 1;
            isDisabled = true;
        } else if (isDisabled && canBeReset){
            resetTimer = true;
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

    public bool isTimerReset(){
        return resetTimer;
    }
}
