using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseCursor : MonoBehaviour
{
    public static bool CursorActivated { get; set; }
    public Texture2D disableCursor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    // Change cursor sprite if hovering over camera
    public void ActivateCursor()
    {
        CursorActivated = true;
        Cursor.SetCursor(disableCursor, new Vector2(0, disableCursor.height), CursorMode.Auto);
    }

    public void DeactivateCursor()
    {
        CursorActivated = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void StartLightningEffect()
    {
        GameObject lightningEffect = gameObject.transform.GetChild(0).gameObject;
        lightningEffect.SetActive(true);
    }

    public void hideLightning()
    {
        GameObject lightningEffect = gameObject.transform.GetChild(0).gameObject;
        lightningEffect.SetActive(false);
    }
}
