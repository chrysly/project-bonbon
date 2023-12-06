using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BonbonAssetManager;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public abstract class DelaySkillAnimation
{
    #if UNITY_EDITOR
    private bool enabled;

    public void OnGUI() {
        using (var scope = new EditorGUILayout.ToggleGroupScope(this.GetType().Name, enabled)) {
            if (enabled && !scope.enabled) InitializeFields();
            enabled = scope.enabled;
            if (!enabled) return;
            InnerGUI();
        }
    }

    protected abstract void InnerGUI();

    protected abstract void InitializeFields();
    #endif
}
