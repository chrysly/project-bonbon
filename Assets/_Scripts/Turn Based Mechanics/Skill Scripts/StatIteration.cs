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
        Reset(si);
    }

    public void Reset(StatIteration si = null) {
        Potency = si != null ? si.Potency : BaseData.BasePotency;
        Defense = si != null ? si.Defense : BaseData.BaseDefense;
        Speed = si != null ? si.Speed : BaseData.BaseSpeed;
        StaminaRegen = si != null ? si.StaminaRegen : BaseData.StaminaRegenRate;
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
        var flatAmount = rawAmount + (Augmentation != null ? Augmentation.damageBoost : 0);
        Debug.Log(Potency);
        return (int) (flatAmount * (1 + Potency / 100f));
    }

    public int ComputeHeal(int rawAmount) {
        return rawAmount + (int) (rawAmount * (Potency / 100f)) + (Augmentation != null ? Augmentation.healBoost : 0);
    }

    public int ComputeDefense(int rawAmount) {
        return (int) (rawAmount * Mathf.Clamp(1 - (Defense / 100f), 0.5f, 1));
    }
}