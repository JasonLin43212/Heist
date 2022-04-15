using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(GuardRouteAction))]
public class GuardRouteActionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 40f;  // 2 lines for this property
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);

        position.y += 2f;
        position.height = 16f;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("actionType"), GUIContent.none);

        position.y += 20f;
        switch ((GuardRouteActionType)property.FindPropertyRelative("actionType").enumValueIndex)
        {
            case GuardRouteActionType.Move:
                EditorGUI.PropertyField(position, property.FindPropertyRelative("moveTarget"));
                break;
            case GuardRouteActionType.Turn:
                EditorGUI.PropertyField(position, property.FindPropertyRelative("turnTarget"));
                break;
            case GuardRouteActionType.Wait:
                EditorGUI.PropertyField(position, property.FindPropertyRelative("waitTime"));
                break;
            default:
                throw new System.Exception("Invalid GuardRouteActionType");
        }
        EditorGUI.EndProperty();
    }
}
