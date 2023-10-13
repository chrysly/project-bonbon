using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRunner : MonoBehaviour {

    [SerializeField] private SkillAnimationMap skillAnimationMap;
    public Dictionary<SkillObject, Dictionary<ActorData, SkillAnimation>> SkillAMap { get; private set; }

    void Awake() {
        SkillAMap = SKAEUtils.ProcessInternalDictionary(skillAnimationMap.animationMap);
    }

    public void OnSkillTrigger(SkillAction skillAction, BonbonBlueprint bonbon = null) {
        try {
            SkillAnimation sa = SkillAMap[skillAction.SkillData][skillAction.Caster.Data];
            skillAction.Caster.GetComponentInChildren<Animator>(true).SetTrigger(sa.AnimationTrigger);
            if (bonbon != null) ; /// Do VFXs
        } catch (KeyNotFoundException) {
            Debug.LogWarning($"Animation Undefined for {skillAction.SkillData.Name} -> {skillAction.Caster.Data.DisplayName}");
        }
    }
}
