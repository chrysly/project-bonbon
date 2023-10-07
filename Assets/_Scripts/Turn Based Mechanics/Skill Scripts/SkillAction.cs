using System.Collections.Generic;
using UnityEngine;

public class SkillAction {

    public SkillObject SkillData { get; private set; }
    public Actor Caster { get; private set; }
    public List<Effect> Effects { get; private set; }
    public int SkillIndex { get; private set; } 

    public SkillAction(SkillObject data, Actor caster, int skillIndex) {
        SkillData = data;
        Caster = caster;
        SkillIndex = skillIndex;
    }

    public AIActionValue ComputeSkillActionValues(Actor target) {
        AIActionValue actionValue = new AIActionValue();
        SkillData.ComputeActionValues(ref actionValue, target.ActiveData);
        actionValue.immediateDamage = target.ActiveData.ComputeDefense(actionValue.immediateDamage);
        actionValue.damageOverTime = target.ActiveData.ComputeDefense(actionValue.damageOverTime);
        return actionValue;
    }

    public void ActivateSkill(Actor[] targets) {
        foreach(Actor target in targets) {
            SkillData.PerformActions(Caster.ActiveData, target);
        }
    }

    public void AugmentSkill(Actor[] targets, SkillAugmentation augment) {
        foreach(Actor target in targets) {
            SkillData.PerformActions(Caster.ActiveData, target);
        }
    }

    public override string ToString() {
        return SkillData.GetSkillName();
    }
}

[System.Serializable]
public class SkillAugmentation {
    public int damageBoost;
    public int healBoost;
    public bool aoeOverride;
}