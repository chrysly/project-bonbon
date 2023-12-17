using System.Collections.Generic;
using UnityEngine;

public class StatIteration {

    public readonly Actor Actor;
    public readonly ActorData BaseData;
    public readonly SkillAugment Augmentation;

    public int Potency { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    public int StaminaRegen { get; private set; }

    public StatIteration(Actor actor, ActorData data) {
        Actor = actor;
        BaseData = data;
        Reset();
    }

    public StatIteration(StatIteration si, SkillAugment sa) {
        Actor = si.Actor;
        BaseData = si.BaseData;
        Augmentation = sa;
        Reset();
    }

    public void Reset() {
        Potency = BaseData.BasePotency;
        Defense = BaseData.BaseDefense;
        Speed = BaseData.BaseSpeed;
        StaminaRegen = BaseData.StaminaRegenRate;
    }

    public void ComputeModifiers(List<PassiveModifier> mods) {
        foreach (PassiveModifier mod in mods) {
            Potency += mod.flatAttack;
            Defense += mod.flatDefense;
            Speed += mod.flatSpeed;
            StaminaRegen += mod.flatStaminaRegen;
        }

        foreach (PassiveModifier mod in mods) {
            Potency += (int) (Potency * mod.percentAttack);
            Defense += (int) (Defense * mod.percentDefense);
            Speed += (int) (Speed * mod.percentSpeed);
        }
    }

    public StatIteration Augment(SkillAugment sa) => new StatIteration(this, sa);

    public int ComputeDamage(int rawAmount) {
        return rawAmount + (int) (rawAmount * (Potency / 100f)) + (Augmentation != null ? Augmentation.damageBoost : 0);
    }

    public int ComputeHeal(int rawAmount) {
        return rawAmount + (int) (rawAmount * (Potency / 100f)) + (Augmentation != null ? Augmentation.healBoost : 0);
    }

    public int ComputeDefense(int rawAmount) {
        return (int) (rawAmount * Mathf.Clamp(1 - (Defense / 100f), 0.5f, 1));
    }
}