using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHandler : StateMachineHandler {

    public event System.Action<ActiveSkillPrep> OnSkillTrigger;

    public void SkillActivate(ActiveSkillPrep SkillPrep) {
        if (SkillPrep.targets.Length > 0) {
            OnSkillTrigger?.Invoke(SkillPrep);
            if (SkillPrep.bonbon == null) SkillPrep.skill.ActivateSkill(SkillPrep.targets);
            else SkillPrep.skill.AugmentSkill(SkillPrep.targets, SkillPrep.bonbon);
        }
    }
}