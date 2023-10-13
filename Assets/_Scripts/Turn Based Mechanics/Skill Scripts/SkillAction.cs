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

    public bool Available => SkillData.staminaCost <= Caster.Stamina;

    private void PayStamina() => Caster.ConsumeStamina(SkillData.staminaCost);

    public void ActivateSkill(Actor[] targets) {
        PayStamina();
        foreach(Actor target in targets) {
            SkillData.PerformActions(Caster.ActiveData, target);
        }
    }

    public void AugmentSkill(Actor[] targets, BonbonObject bonbon) {
        PayStamina();
        SkillAugment augment = bonbon.Data.augmentData;
        /// Ensure that the skill applies effects if it originally didn't;
        var aea = new ApplyEffectsAction();
        if (augment.augmentEffects != null
            && !SkillData.immediateActions.Contains(aea)) SkillData.immediateActions.Add(aea);
        /// Perform the actions on the caster with new computations;
        foreach (Actor target in targets) SkillData.PerformActions(Caster.ActiveData, target, augment);
    }

    public override string ToString() {
        return SkillData.GetSkillName();
    }
}

[System.Serializable]
public class SkillAugment {
    /// <summary> Base boost for damaging abilities; </summary>
    public int damageBoost;
    /// <summary> Base boost for healing abilities; </summary>
    public int healBoost;
    /// <summary> Effects applied to targets through the Augment; </summary>
    public List<EffectBlueprint> augmentEffects;

    /// <summary> Actions performed on the caster by the Augment; </summary>
    public List<ImmediateAction> immediateActions;
    /// <summary> Bonbon effect; </summary>
    public EffectBlueprint bonbonEffect;
    /// <summary> New AoE protocol through the augment; </summary>
    public bool aoeOverride;
}