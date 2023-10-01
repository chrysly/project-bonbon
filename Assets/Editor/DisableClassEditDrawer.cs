using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisableClassEditAttribute))]
public class DisableClassEditDrawer : PropertyDrawer
{
    bool isUnfolded = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        isUnfolded = EditorGUI.BeginFoldoutHeaderGroup(position, isUnfolded, label);
        if (isUnfolded) {
            bool previuousGUIState = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.indentLevel++;

            float currentY = position.y;
            foreach (SerializedProperty childProperty in property) {
                currentY += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, currentY, position.width, position.height), childProperty);
            }

            EditorGUI.indentLevel--;
            GUI.enabled = previuousGUIState;
        }
        EditorGUI.EndFoldoutHeaderGroup();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        int propertyCount = isUnfolded ? property.CountInProperty() : 1;
        return EditorGUIUtility.singleLineHeight * propertyCount;
    }
}
