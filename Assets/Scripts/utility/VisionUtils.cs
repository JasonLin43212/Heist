using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VisionUtils
{
    // Check if the guard can see a player at the given position. Returns Nullable<Vector2> of the new target position.
    public static Vector2? CanSeePlayer(GameObject visionConeObject, Rigidbody2D playerRigidbody)
    {
        PolygonCollider2D visionConeCollider = visionConeObject.GetComponent<PolygonCollider2D>();
        if (playerRigidbody.IsTouching(visionConeCollider))
        {
            Vector2 sightedPosition = visionConeCollider.ClosestPoint(playerRigidbody.position);
            return sightedPosition;
        }
        return null;
    }

    public static void DrawBetterVisionCone(
        Transform originTransform,
        Mesh visionConeMesh,
        GameObject visionConeObject,
        int visionConeResolution,
        float visionAngle,
        float visionRange
    )
    {
        // Layer mask avoids "clickable" 
        LayerMask raycastLayerMask = ~LayerMask.GetMask("Ignore Raycast", "Clickable");

        // Get vertices of mesh
        Vector2[] colliderPoints = new Vector2[visionConeResolution + 2];
        colliderPoints[0] = new Vector2(0, 0);
        float angle = -visionAngle;
        float arcLength = 2 * visionAngle;
        for (int i = 0; i <= visionConeResolution; i++)
        {
            float worldAngle = originTransform.rotation.eulerAngles.z + angle;
            Vector2 raycastDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * worldAngle), Mathf.Sin(Mathf.Deg2Rad * worldAngle));
            RaycastHit2D raycastHit = Physics2D.Raycast((Vector2)originTransform.position, raycastDirection, visionRange, raycastLayerMask);
            float raycastRange = (raycastHit.collider != null) ? raycastHit.distance : visionRange;

            Vector2 vertexDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
            colliderPoints[i + 1] = vertexDirection * raycastRange;

            angle += (arcLength / visionConeResolution);
        }

        Vector3[] meshPoints = System.Array.ConvertAll<Vector2, Vector3>(colliderPoints, point => point);

        // Calculate triangles array
        int[] triangles = new int[visionConeResolution * 3];
        for (int i = 0; i < visionConeResolution; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i + 2;
        }

        // Set mesh
        visionConeMesh.Clear();
        visionConeMesh.vertices = meshPoints;
        visionConeMesh.uv = new Vector2[visionConeResolution + 2];
        visionConeMesh.triangles = triangles;
        visionConeMesh.RecalculateNormals();
        visionConeMesh.RecalculateBounds();

        // Set collider points
        PolygonCollider2D collider = visionConeObject.GetComponent<PolygonCollider2D>();
        collider.SetPath(0, colliderPoints);
    }

    public static void UpdateVisionConeColor(GameObject visionConeObject, float suspicionTime, float secondsToCatch)
    {
        float alertRatio = Mathf.Min(suspicionTime / secondsToCatch, 1f);  // between 0 and 1
        float red = Mathf.Min(alertRatio * 2, 1f);  // linear from 0 to 0.5
        float green = 1f - Mathf.Max(alertRatio * 2 - 1, 0f);  // linear from 0.5 to 1
        visionConeObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(red, green, 0f, 83f / 255f));
    }

    /*
     * LEGACY VISION SYSTEM 
     * ====================
     * These functions were included in GuardBehaviour as the naive but faster cone system.
     * They are being phased out, but I'll leave the code here just in case we want to revert to it.
     */
    // private void CastVisionRay()
    // {
    //     Vector2 direction = (Vector2)directionMarkerTransform.position - myRigidbody.position;
    //     RaycastHit2D raycast = Physics2D.Raycast(myRigidbody.position, direction, visionRange, raycastLayerMask);
    //     blockedVisionRange = (raycast.collider != null) ? raycast.distance : visionRange;
    // }

    // private void DrawVisionCone()
    // {
    //     if (Mathf.Abs(drawnVisionRange - blockedVisionRange) < 1e-5 && Mathf.Abs(drawnVisionAngle - visionAngle) < 1e-5) return;
    //     // Don't redraw unless necessary

    //     drawnVisionRange = blockedVisionRange;
    //     drawnVisionAngle = visionAngle;

    //     visionConeMesh.Clear();

    //     // Get vertices of mesh
    //     Vector2[] colliderPoints = new Vector2[visionConeResolution + 2];
    //     colliderPoints[0] = new Vector2(0, 0);
    //     float angle = -visionAngle;
    //     float arcLength = 2 * visionAngle;
    //     for (int i = 0; i <= visionConeResolution; i++)
    //     {
    //         float x = Mathf.Cos(Mathf.Deg2Rad * angle) * blockedVisionRange;
    //         float y = Mathf.Sin(Mathf.Deg2Rad * angle) * blockedVisionRange;

    //         colliderPoints[i + 1] = new Vector2(x, y);

    //         angle += (arcLength / visionConeResolution);
    //     }
    //     Vector3[] meshPoints = System.Array.ConvertAll<Vector2, Vector3>(colliderPoints, point => point);

    //     // Calculate triangles array
    //     int[] triangles = new int[visionConeResolution * 3];
    //     for (int i = 0; i < visionConeResolution; i++)
    //     {
    //         triangles[3 * i] = 0;
    //         triangles[3 * i + 1] = i + 1;
    //         triangles[3 * i + 2] = i + 2;
    //     }

    //     // Set mesh
    //     visionConeMesh.vertices = meshPoints;
    //     visionConeMesh.uv = new Vector2[visionConeResolution + 2];
    //     visionConeMesh.triangles = triangles;
    //     visionConeMesh.RecalculateNormals();
    //     visionConeMesh.RecalculateBounds();

    //     // Set collider points
    //     PolygonCollider2D collider = visionConeObject.GetComponent<PolygonCollider2D>();
    //     collider.SetPath(0, colliderPoints);
    // }
}
