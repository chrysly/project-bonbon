using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class PercentTrigger
{
    public class CompareByTriggerTime : IComparer<PercentTrigger> {
        public int Compare(PercentTrigger x, PercentTrigger y) {
            return Math.Sign(x.triggerTime - y.triggerTime);
        }
    }

    [SerializeField] private float triggerTime;
    public float TriggerTime => triggerTime;
    [SerializeField] private float percentMultiplier;
    public float Multiplier => percentMultiplier / 100;

    public PercentTrigger(float triggerTime = 0, float percentMultiplier = 0) {
        this.triggerTime = triggerTime;
        this.percentMultiplier = percentMultiplier;
    }


#if UNITY_EDITOR
    public void OnGUI(string description = "") {
        using (new EditorGUILayout.HorizontalScope()) {
            percentMultiplier = Mathf.Clamp(EditorGUILayout.FloatField("Apply", percentMultiplier), 0, 100);
            EditorGUILayout.LabelField("%", GUILayout.Width(24));
            triggerTime = Mathf.Max(EditorGUILayout.FloatField($"{description} at", triggerTime), 0);
            EditorGUILayout.LabelField("s", GUILayout.Width(24));
        }
    }
#endif
}
