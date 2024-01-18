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

    [SerializeReference] private List<DelaySkillAnimation> delaySkills;
    public List<DelaySkillAnimation> DelaySkills {
        get {
            if (delaySkills == null) {
                delaySkills = new();
                delaySkills.Add(new DelaySkillDamageAnimation());
                delaySkills.Add(new DelaySkillEffectAnimation());
                delaySkills.Add(new DelaySkillHealAnimation());
            } return delaySkills;
        }
    }

    [SerializeField] private CameraAnimationPackage cap;
    public CameraAnimationPackage CameraAnimationPackage => cap;

    [SerializeField] private VFXAnimationPackage baseSkillVFX;
    public VFXAnimationPackage BaseSkillVFX => baseSkillVFX;

    [SerializeField] private VFXAnimationPackage bonbonSkillVFX;
    public VFXAnimationPackage BonbonSkillVFX => bonbonSkillVFX != null ? bonbonSkillVFX : baseSkillVFX;

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

    public void SetBaseSkillVFX(VFXAnimationPackage baseSkillVFX) {
        this.baseSkillVFX = baseSkillVFX;
    }

    public void SetBonbonSkillVFX(VFXAnimationPackage bonbonSkillVFX) {
        this.bonbonSkillVFX = bonbonSkillVFX;
    }

    public void SetCap(CameraAnimationPackage cap) {
        this.cap = cap;
    }

    #endif
}