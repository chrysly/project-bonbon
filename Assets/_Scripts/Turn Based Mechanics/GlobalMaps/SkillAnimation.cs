using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

[System.Serializable]
public class SkillAnimation {

    public string AnimationTrigger { get; private set; }
    public GameObject VFXPrefab { get; private set; }

#if UNITY_EDITOR

    /// Animation Editor ///
    public void SetAnimationTrigger(string trigger) => AnimationTrigger = trigger;

    public void SetActorAnimator(AnimatorController controller) {
        triggerIndex = 0;
        triggers = null;
        actorAnimator = controller;
    }

    public AnimatorController actorAnimator;

    public int triggerIndex;

    public string[] Triggers {
        get {
            if (triggers == null && actorAnimator != null) {

                List<string> tempTriggers = new List<string>();
                for (int i = 0; i < actorAnimator.parameters.Length; i++) {
                    if (actorAnimator.parameters[i].type == AnimatorControllerParameterType.Trigger) {
                        tempTriggers.Add(actorAnimator.parameters[i].name);
                    }
                } triggers = tempTriggers.ToArray();
            } return triggers;
        }
    } private string[] triggers;

    /// VFX Editor ///

    public void SetVFXPrefab(GameObject prefab) {
        CleanDelayEditor();
        VFXPrefab = prefab;
    }

    public UnityEditor.Editor DelayScriptEditor {
        get {
            if (delayScriptEditor is null && VFXPrefab != null) {
                SlashTest delayComponent = VFXPrefab.GetComponentInChildren<SlashTest>(true);
                if (delayComponent != null) delayScriptEditor = UnityEditor.Editor.CreateEditor(delayComponent);
            } return delayScriptEditor;
        }
    } private UnityEditor.Editor delayScriptEditor;

    public void CleanDelayEditor() {
        Object.DestroyImmediate(DelayScriptEditor);
        delayScriptEditor = null;
    }

    #endif
}