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

    public AIActionValue ComputeSkillActionValues(Actor actor) {
        AIActionValue actionValue = new AIActionValue();
        SkillData.ComputeActionValues(ref actionValue, actor.ActiveData);
        actionValue.immediateDamage = actor.ActiveData.ComputeDefense(actionValue.immediateDamage);
        actionValue.damageOverTime = actor.ActiveData.ComputeDefense(actionValue.damageOverTime);
        return actionValue;
    }

    public void ActivateSkill(Actor[] targets) {
        foreach(Actor target in targets) {
            SkillData.PerformActions(Caster.ActiveData, target);
        }
    }

    public override string ToString() {
        return SkillData.GetSkillName();
    }
}

public class StatIteration {

    public readonly Actor Actor;
    private readonly ActorData baseData;

    public int Potency { get; private set; }
    public int Defense { get; private set; }
    public int StaminaRegen { get; private set; }

    public StatIteration(Actor actor, ActorData data) {
        Actor = actor;
        baseData = data;
        Reset();
    }

    public void Reset() {
        Potency = baseData.BasePotency;
        Defense = baseData.BaseDefense;
        StaminaRegen = baseData.StaminaRegenRate;
    }

    public void ComputeModifiers(List<PassiveModifier> mods) {
        foreach (PassiveModifier mod in mods) {
            Potency = (int) (mod.attackModifier * Potency);
            Defense = (int) (mod.defenseModifier * Defense);
            StaminaRegen = (int) (mod.staminaRegenModifier + StaminaRegen);
        }
    }

    public int ComputePotency(int rawAmount) {
        return rawAmount + rawAmount * (Potency / 100);
    }

    public int ComputeDefense(int rawAmount) {
        return rawAmount * (1 - (Defense / 100));
    }
}