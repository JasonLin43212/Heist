using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GuardBehaviour), true)]
[CanEditMultipleObjects]
public class GuardBehaviourEditor : Editor
{
    public bool displayConstants, displayRoute, displayReferences, displayToggles;
    private List<string> constantFields, referenceFields, toggleFields;

    void OnEnable()
    {
        constantFields = new List<string> {
            "visionRange", "visionAngle",
            "targetChaseDistanceRatio",
            "moveSpeed", "chaseSpeed",
            "secondsToCatch", "catchRateMultiplierMin", "catchRateMultiplierMax", "suspicionDecreaseRate",
            "visionConeResolution", "ROTATION_SPEED"
        };
        referenceFields = new List<string> {
            "directionMarkerTransform", "visionConeObject", "alertMarkerObject", 
            "alertSpriteMaskObject", "timerText", "canvasTransform", "guardSounds", "animator",
            "guardSprite"
        };
        toggleFields = new List<string> {
            "enableMove", "strictChasing"
        };
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("debugText"));

        serializedObject.Update();

        // Constants
        displayConstants = EditorGUILayout.Toggle("Display Constants", displayConstants);
        if (displayConstants)
        {
            IncreaseIndent();
            foreach (string fieldName in constantFields)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName));
            }
            DecreaseIndent();
        }
        EditorGUILayout.Separator();

        // Route
        displayRoute = true;  // EditorGUILayout.Toggle("Display Route", displayRoute);
        if (displayRoute)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultRouteActions"), true);
        }
        EditorGUILayout.Separator();

        // Object References
        displayReferences = EditorGUILayout.Toggle("Display Object References", displayReferences);
        if (displayReferences)
        {
            IncreaseIndent();
            foreach (string fieldName in referenceFields)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName));
            }
            DecreaseIndent();
        }
        EditorGUILayout.Separator();

        // Toggles
        displayToggles = EditorGUILayout.Toggle("Display General Toggles", displayToggles);
        if (displayToggles)
        {
            IncreaseIndent();
            foreach (string fieldName in toggleFields)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName));
            }
            DecreaseIndent();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void IncreaseIndent(int indent = 1)
    {
        EditorGUI.indentLevel += indent;
    }

    private void DecreaseIndent(int indent = 1)
    {
        EditorGUI.indentLevel -= indent;
    }
}
