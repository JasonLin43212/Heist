using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UnityEditor.U2D.Animation.CharacterData))]
public class CharacterListDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 60f;  // 2 lines for this property
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);

        position.y += 2f;
        position.height = 16f;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("name"));

        position.y += 20f;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("displayName"));

        position.y += 20f;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("sprite"));
        EditorGUI.EndProperty();
    }
}
