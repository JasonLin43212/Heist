using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardDebugger : MonoBehaviour
{
    public GuardBehaviour target;

    void Start()
    {
        target.enableDebugLogging = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetPosition = target.targetPosition;
        transform.position = new Vector3(targetPosition.x, targetPosition.y, 0);
    }
}
