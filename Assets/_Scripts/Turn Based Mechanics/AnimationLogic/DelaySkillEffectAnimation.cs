using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using CJUtils;
#endif

[System.Serializable]
public class DelaySkillEffectAnimation : DelaySkillAnimation
{
    public float triggerTime;

    #if UNITY_EDITOR
    protected override void InnerGUI() {
        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
            triggerTime = Mathf.Max(EditorGUILayout.FloatField("Apply all effects at", triggerTime), 0);
            EditorGUILayout.LabelField("s", GUILayout.Width(24));
        }
    }

    protected override void InitializeFields() {
        triggerTime = 0;
    }
    #endif
}
