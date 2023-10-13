using System.Collections.Generic;

public class StatIteration {

    public readonly Actor Actor;
    private readonly ActorData baseData;

    public int Potency { get; private set; }
    public int Defense { get; private set; }
    public int Speed { get; private set; }
    public float StaminaRegen { get; private set; }

    public StatIteration(Actor actor, ActorData data) {
        Actor = actor;
        baseData = data;
        Reset();
    }

    public void Reset() {
        Potency = baseData.BasePotency;
        Defense = baseData.BaseDefense;
        Speed = baseData.BaseSpeed;
        StaminaRegen = baseData.StaminaRegenRate;
    }

    public void ComputeModifiers(List<PassiveModifier> mods) {
        foreach (PassiveModifier mod in mods) {
            Potency += mod.flatAttack;
            Defense += mod.flatDefense;
            Speed += mod.flatSpeed;
            StaminaRegen += mod.percentStaminaRegen;
        }

        foreach (PassiveModifier mod in mods) {
            Potency += (int) (Potency * mod.percentAttack);
            Defense += (int) (Defense * mod.percentDefense);
            Speed += (int) (Speed * mod.percentSpeed);
        }
    }

    public int ComputePotency(int rawAmount) {
        return rawAmount + rawAmount * (Potency / 100);
    }

    public int ComputeDefense(int rawAmount) {
        return rawAmount * (1 - (Defense / 100));
    }
}