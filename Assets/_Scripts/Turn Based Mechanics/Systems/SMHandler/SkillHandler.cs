using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHandler : StateMachineHandler {

    public event System.Action<ActiveSkillPrep> OnSkillTrigger;

    private ActiveSkillPrep skillPrep;
    public ActiveSkillPrep SkillPrep {
        get {
            if (skillPrep == null) skillPrep = new ActiveSkillPrep();
            return skillPrep;
        }
    }

    public void SkillUpdate(SkillAction skillAction, Actor[] targets) {
        SkillPrep.skill = skillAction;
        SkillPrep.targets = targets;
    }

    public void SkillUpdate(SkillAction skillAction) => SkillPrep.skill = skillAction;
    public void SkillUpdate(Actor[] targets) => SkillPrep.targets = targets;
    public void SkillUpdate(BonbonObject bonbon) => SkillPrep.bonbon = bonbon;

    public void SkillActivate() {
        if (SkillPrep.targets.Length > 0) {
            if (SkillPrep.bonbon == null) SkillPrep.skill.ActivateSkill(SkillPrep.targets);
            else SkillPrep.skill.AugmentSkill(SkillPrep.targets, SkillPrep.bonbon);
            OnSkillTrigger?.Invoke(skillPrep);
        }
    }

    public void SkillReset() => skillPrep = new ActiveSkillPrep();
}