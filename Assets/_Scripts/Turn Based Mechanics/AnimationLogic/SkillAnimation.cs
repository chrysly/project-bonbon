using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

[System.Serializable]
public class SkillAnimation {

    [SerializeField] private string animationTrigger;
    public string AnimationTrigger => animationTrigger;

    [SerializeField] private float animationDuration;
    public float AnimationDuration => animationDuration;

    [SerializeField] private float hitDelay;
    public float HitDelay => hitDelay;

    [SerializeField] private GameObject vfxPrefab;
    public GameObject VFXPrefab => vfxPrefab;

#if UNITY_EDITOR

    /// Animation Editor ///
    public void SetAnimationTrigger(string trigger) => animationTrigger = trigger;

    public void SetAnimationDuration(float duration) => animationDuration = duration;
    public void SetHitDelay(float delay) => hitDelay = delay;

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
        vfxPrefab = prefab;
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