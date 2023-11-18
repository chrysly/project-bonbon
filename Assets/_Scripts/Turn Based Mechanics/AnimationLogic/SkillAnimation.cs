using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

[System.Serializable]
public class SkillAnimation {

    [SerializeField] private string animationTrigger;
    public string AnimationTrigger => animationTrigger;

    [SerializeField] private float animationDuration;
    public float AnimationDuration => animationDuration;

    [SerializeField] private GameObject vfxPrefab;
    public GameObject VFXPrefab => vfxPrefab;
    [SerializeReference] private List<DelaySkillAnimation> delaySkills = DelaySkillAnimation.subTypes.Select(type => (DelaySkillAnimation) System.Activator.CreateInstance(type)).ToList();
    public List<DelaySkillAnimation> DelaySkills => delaySkills;

    [SerializeField] private CameraAnimationPackage cap;
    public CameraAnimationPackage CameraAnimationPackage => cap;

#if UNITY_EDITOR

    /// Animation Editor ///
    public void SetAnimationTrigger(string trigger) => animationTrigger = trigger;

    public void SetAnimationDuration(float duration) => animationDuration = duration;

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

    public void SetCap(CameraAnimationPackage cap) {
        this.cap = cap;
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