using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif 

/// <summary>
/// Representation of an active Effect in an Actor;
/// </summary>

public class Effect : ScriptableObject {

    [SerializeField] private new string name;

    /// <summary> Duration of the effect in turns; </summary>
    [SerializeField] private int duration;

    /// <summary> Modifiers applied while the effect is active; </summary>
    public PassiveModifier modifiers;
    /// <summary> Actions carried at the beginning of every turn; </summary>
    public List<ImmediateAction> actions;
    /// <summary> Stats of the original effector; </summary>
    private StatIteration originStats;

    /// <summary>
    /// Define a set of originStats;
    /// </summary>
    /// <param name="originStats"> Stats of the Actor applying the effects; </param>
    public void DefineEffectOrigin(StatIteration originStats) {
        this.originStats = originStats;
    }

    /// <summary>
    /// Use the actions designated in the Effect's list;
    /// </summary>
    /// <param name="actor"> Actor affected by this effect; </param>
    public void PerformActions(Actor actor) {
        foreach (ImmediateAction action in actions) action.Use(originStats, actor);
    }

    /// <summary>
    /// Compute action values for each element of the effect;
    /// </summary>
    /// <param name="actionValue"> AI Value bundle passed down for data collection; </param>
    public void ComputeEffectValue(ref AIActionValue actionValue, StatIteration casterData) {
        AIActionValue compoundValue = new AIActionValue();
        foreach (ImmediateAction action in actions) {
            action.ComputeActionValue(ref compoundValue, casterData);
        } actionValue.damageOverTime = compoundValue.immediateDamage * duration;
        actionValue.healOverTime = compoundValue.immediateHeal * duration;
    }

    /// <summary>
    /// Check and decrement the skills duration;
    /// </summary>
    /// <returns> True if the effect has no turns remaining, and thus should expire; </returns>
    public bool IsSpent() {
        duration--;
        return duration == 0;
    }
}

/// <summary>
/// Bundle class for stat modifiers;
/// <br></br> Belongs in Actors and Bonbons;
/// </summary>
[System.Serializable]
public class PassiveModifier {

    /// <summary> Multiplier for the Actor attack attribute; </summary>
    public float attackModifier;
    /// <summary> Multiplier for the Actor heal cast attribute; </summary>
    public float healCastModifier;
    /// <summary> Multiplier for the Actor attack attribute; </summary>
    public float defenseModifier;
    /// <summary> Multiplier for the Actor stamina regen attribute; </summary>
    public float staminaRegenModifier;

    //public float speedModifier;
    //public float evasionModifier;
}

/// <summary>
/// Generic definition for an action;
/// </summary>
public abstract class ImmediateAction : ScriptableObject {

    /// <summary>
    /// Override to implement AI value yield;
    /// </summary>
    /// <param name="actionValue"> Value bundle to operate on; </param>
    public virtual void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) { }

    /// <summary>
    /// Trigger the action encompassed by this object;
    /// </summary>
    /// <param name="activeData"> Active data of the casting actor; </param>
    /// <param name="target"> Target actor for the skill; </param>
    public abstract void Use(StatIteration activeData, Actor target = null);
}

/// <summary>
/// A bundle of AI values from skill actions;
/// </summary>
public class AIActionValue {

    /// <summary> Damage triggered immediately upon skill use; </summary>
    public int immediateDamage;
    /// <summary> Heal triggered immediately upon skill use; </summary>
    public int immediateHeal;
    /// <summary> Damage distributed over several turns; </summary>
    public int damageOverTime;
    /// <summary> Heal distributed over several turns; </summary>
    public int healOverTime;
    /// <summary> Special skill priority; </summary>
    public int specialValue;
}

/// <summary>
/// Action to deplete the hitpoints of a target;
/// </summary>
public class DamageAction : ImmediateAction {

    /// <summary> Amount to deduct from the target's hitpoint pool; </summary>
    [SerializeField] private int damageAmount;

    public static DamageAction CreateInstance(int damageAmount) {
        DamageAction action = CreateInstance<DamageAction>();
        action.damageAmount = damageAmount;
        return action;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        int computedDamage = casterData.ComputePotency(damageAmount);
        actionValue.immediateDamage -= computedDamage;
    }

    public override void Use(StatIteration activeData, Actor target) {
        int computedDamage = activeData.ComputePotency(damageAmount);
        target.DepleteHitpoints(computedDamage);
    }

    #if UNITY_EDITOR

    /// <summary>
    /// EDITOR-ONLY: Change the damage amount in the ScriptableObject;
    /// </summary>
    /// <param name="amount"> New damage amount; </param>
    public void Modify(int amount) {
        damageAmount = amount;
        EditorUtility.SetDirty(this);
    }

    #endif
}

/// <summary>
/// Action to replenish the hitpoints of a target;
/// </summary>
public class HealAction : ImmediateAction {

    /// <summary> Amount to replenish in the target's hitpoint pool; </summary>
    [SerializeField] private int healAmount;

    public static HealAction CreateInstance(int healAmount) {
        HealAction action = CreateInstance<HealAction>();
        action.healAmount = healAmount;
        return action;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        int computedHeal = casterData.ComputePotency(healAmount);
        actionValue.immediateHeal += computedHeal;
    }

    public override void Use(StatIteration activeData, Actor target) {
        int computedHeal = activeData.ComputePotency(healAmount);
        target.RestoreHitpoints(computedHeal);
    }

    #if UNITY_EDITOR

    /// <summary>
    /// EDITOR-ONLY: Change the heal amount in the ScriptableObject;
    /// </summary>
    /// <param name="amount"> New heal amount; </param>
    public void Modify(int amount) {
        healAmount = amount;
        EditorUtility.SetDirty(this);
    }

    #endif
}

/// <summary>
/// Action to deplete the hitpoints of the caster without target selection;
/// </summary>
public class SelfDamageAction : ImmediateAction {

    /// <summary> Amount to deduct from the caster's hitpoint pool; </summary>
    [SerializeField] private int damageAmount;

    public static SelfDamageAction CreateInstance(int damageAmount) {
        SelfDamageAction action = CreateInstance<SelfDamageAction>();
        action.damageAmount = damageAmount;
        return action;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        actionValue.immediateHeal -= damageAmount;
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        activeData.Actor.DepleteHitpoints(damageAmount);
    }

    #if UNITY_EDITOR

    /// <summary>
    /// EDITOR-ONLY: Change the damage amount in the ScriptableObject;
    /// </summary>
    /// <param name="amount"> New damage amount; </param>
    public void Modify(int amount) {
        damageAmount = amount;
        EditorUtility.SetDirty(this);
    }

    #endif
}

/// <summary>
/// Action to replenish the hitpoints of the caster without target selection;
/// </summary>
public class SelfHealAction : ImmediateAction {

    /// <summary> Amount to replenish in the caster's hitpoint pool; </summary>
    [SerializeField] private int healAmount;

    public static SelfHealAction CreateInstance(int healAmount) {
        SelfHealAction action = CreateInstance<SelfHealAction>();
        action.healAmount = healAmount;
        return action;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterStats) {
        int computedHeal = casterStats.ComputePotency(healAmount);
        actionValue.immediateHeal += computedHeal;
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        int computedHeal = activeData.ComputePotency(healAmount);
        activeData.Actor.RestoreHitpoints(computedHeal);
    }

    #if UNITY_EDITOR

    /// <summary>
    /// EDITOR-ONLY: Change the heal amount in the ScriptableObject;
    /// </summary>
    /// <param name="amount"> New heal amount; </param>
    public void Modify(int amount) {
        healAmount = amount;
        EditorUtility.SetDirty(this);
    }

    #endif
}

/// <summary>
/// Action to change an actor's stamina;
/// <br></br> Note: I could split this into "Deplete" and "Replenish" Actions on demand;
/// </summary>
public class StaminaChangeAction : ImmediateAction {

    /// <summary> Stamina added (+) or deducted (-) from the target's stamina pool; </summary>
    [SerializeField] private int staminaAmount;

    public static StaminaChangeAction CreateInstance(int staminaAmount) {
        StaminaChangeAction action = CreateInstance<StaminaChangeAction>();
        action.staminaAmount = staminaAmount;
        return action;
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        target.RefundStamina(staminaAmount);
    }


    /// <summary>
    /// EDITOR-ONLY: Change the stamina amount in the ScriptableObject;
    /// </summary>
    /// <param name="amount"> New stamina amount; </param>
    public void Modify(int amount) {
        staminaAmount = amount;
        EditorUtility.SetDirty(this);
    }
}

public class SkipTurnAction : ImmediateAction {

    public static SkipTurnAction CreateInstance() {
        return CreateInstance<SkipTurnAction>();
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        // Skip Turn;
    }
}

/// <summary>
/// Action to Apply Effects to the given targets;
/// </summary>
public class ApplyEffectsAction : ImmediateAction {

    /// <summary> List of effects applied by the action; </summary>
    [SerializeField] private List<Effect> effects;

    public static ApplyEffectsAction CreateInstance(List<Effect> effects) {
        ApplyEffectsAction action = CreateInstance<ApplyEffectsAction>();
        if (effects == null) effects = new List<Effect>();
        action.effects = effects;
        return action;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        foreach (Effect effect in effects) effect.ComputeEffectValue(ref actionValue, casterData);
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        List<Effect> appliedEffectList = new List<Effect>(effects);
        foreach (Effect effect in appliedEffectList) effect.DefineEffectOrigin(activeData);
        target.ApplyEffects(appliedEffectList);
    }

    #if UNITY_EDITOR

    /// <summary>
    /// EDITOR-ONLY: Add an Effect to the effect list in the ScriptableObject;
    /// </summary>
    /// <param name="effect"> Effect to add; </param>
    public void AddEffect(Effect effect) {
        effects.Add(effect);
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// EDITOR-ONLY: Remove an Effect from the effect list in the ScriptableObject;
    /// </summary>
    /// <param name="effectIndex"> Index of the effect to remove; </param>
    public void RemoveEffect(int effectIndex) {
        effects.RemoveAt(effectIndex);
        EditorUtility.SetDirty(this);
    }

    #endif
}