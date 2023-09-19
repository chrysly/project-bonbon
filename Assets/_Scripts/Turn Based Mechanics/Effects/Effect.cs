using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : ScriptableObject {

    [SerializeField] private new string name;
    [SerializeField] private int duration;

    public EffectModifier modifiers;
    public List<EffectAction> actions;

    public void PerformActions 

    public bool IsSpent() {
        duration--;
        return duration == 0;
    }
}

[System.Serializable]
public class EffectModifier {

    public float attackModifier;
    public float healModifier;
    public float defenseModifier;
    public float staminaRegenModifier;
    //public float speedModifier;
    //public float evasionModifier;
}

public abstract class EffectAction : ScriptableObject {

    public abstract void Use(StatIteration activeData, Actor target = null);
}

public class DamageAction : EffectAction {

    [SerializeField] private int damageAmount;

    public static DamageAction CreateInstance(int damageAmount) {
        DamageAction action = CreateInstance<DamageAction>();
        action.damageAmount = damageAmount;
        return action;
    }

    public override void Use(StatIteration activeData, Actor target) {
        target.DepleteHitpoints((activeData.Attack / 100) * damageAmount);
    }

    #if UNITY_EDITOR

    public void Update(int amount) {
        damageAmount = amount;
    }

    #endif
}

public class HealAction : EffectAction {

    [SerializeField] private int healAmount;

    public static HealAction CreateInstance(int healAmount) {
        HealAction action = CreateInstance<HealAction>();
        action.healAmount = healAmount;
        return action;
    }

    public override void Use(StatIteration activeData, Actor target) {
        target.RestoreHitpoints(healAmount);
    }

    #if UNITY_EDITOR

    public void Update(int amount) {
        healAmount = amount;
    }

    #endif
}

public class StaminaChangeAction : EffectAction {

    [SerializeField] private int staminaAmount;

    public static StaminaChangeAction CreateInstance(int staminaAmount) {
        StaminaChangeAction action = CreateInstance<StaminaChangeAction>();
        action.staminaAmount = staminaAmount;
        return action;
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        target.RefundStamina(staminaAmount);
    }
}

public class SkipTurnAction : EffectAction {

    public SkipTurnAction CreateInstance() {
        return CreateInstance<SkipTurnAction>();
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        // Skip Turn;
    }
}

public class ApplyEffectsAction : EffectAction {

    [SerializeField] private List<Effect> effects;

    public ApplyEffectsAction CreateInstance(List<Effect> effects) {
        ApplyEffectsAction action = CreateInstance<ApplyEffectsAction>();
        if (effects == null) effects = new List<Effect>();
        action.effects = effects;
        return action;
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        target.ApplyEffects(effects);
    }
}