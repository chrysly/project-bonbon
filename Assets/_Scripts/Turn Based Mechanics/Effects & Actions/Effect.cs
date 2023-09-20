using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instantiable effect class;
/// </summary>
public class Effect {

    public EffectBlueprint Data { get; private set; }
    /// <summary> Stats of the original effector; </summary>
    public int duration;
    public PassiveModifier modifiers => Data.modifiers;
    public List<ImmediateAction> actions => Data.actions;

    private StatIteration originStats;

    /// <summary>
    /// Constructor for an effect;
    /// </summary>
    /// <param name="originStats"> Stats of the Actor applying the effects; </param>
    public Effect(EffectBlueprint data, StatIteration originStats) {
        Data = data;
        duration = data.duration;
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
    /// Check and decrement the skills duration;
    /// </summary>
    /// <returns> True if the effect has no turns remaining, and thus should expire; </returns>
    public bool IsSpent() {
        duration--;
        return duration == 0;
    }
}

/// <summary>
/// Generic definition for an action;
/// </summary>
[Serializable]
public abstract class ImmediateAction {

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

    #if UNITY_EDITOR

    public void DrawProperty() {
        using (new UnityEditor.EditorGUILayout.HorizontalScope(CJUtils.UIStyles.WindowBox)) {
            UnityEditor.EditorGUILayout.IntField(0);
        }
    }

    #endif
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
[Serializable]
public class DamageAction : ImmediateAction {

    /// <summary> Amount to deduct from the target's hitpoint pool; </summary>
    [SerializeField] private int damageAmount;

    public DamageAction(int damageAmount) {
        this.damageAmount = damageAmount;
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
    }

    #endif
}

/// <summary>
/// Action to replenish the hitpoints of a target;
/// </summary>
[Serializable]
public class HealAction : ImmediateAction {

    /// <summary> Amount to replenish in the target's hitpoint pool; </summary>
    [SerializeField] private int healAmount;

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
    }

    #endif
}

/// <summary>
/// Action to deplete the hitpoints of the caster without target selection;
/// </summary>
[Serializable]
public class SelfDamageAction : ImmediateAction {

    /// <summary> Amount to deduct from the caster's hitpoint pool; </summary>
    [SerializeField] private int damageAmount;

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
    }

    #endif
}

/// <summary>
/// Action to replenish the hitpoints of the caster without target selection;
/// </summary>
[Serializable]
public class SelfHealAction : ImmediateAction {

    /// <summary> Amount to replenish in the caster's hitpoint pool; </summary>
    [SerializeField] private int healAmount;

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
    }

    #endif
}

/// <summary>
/// Action to change an actor's stamina;
/// <br></br> Note: I could split this into "Deplete" and "Replenish" Actions on demand;
/// </summary>
[Serializable]
public class StaminaChangeAction : ImmediateAction {

    /// <summary> Stamina added (+) or deducted (-) from the target's stamina pool; </summary>
    [SerializeField] private int staminaAmount;

    public override void Use(StatIteration activeData, Actor target = null) {
        target.RefundStamina(staminaAmount);
    }


    /// <summary>
    /// EDITOR-ONLY: Change the stamina amount in the ScriptableObject;
    /// </summary>
    /// <param name="amount"> New stamina amount; </param>
    public void Modify(int amount) {
        staminaAmount = amount;
    }
}

[Serializable]
public class SkipTurnAction : ImmediateAction {

    public override void Use(StatIteration activeData, Actor target = null) {
        // Skip Turn;
    }
}

/// <summary>
/// Action to Apply Effects to the given targets;
/// </summary>
[Serializable]
public class ApplyEffectsAction : ImmediateAction {

    /// <summary> List of effects applied by the action; </summary>
    [SerializeField] private List<EffectBlueprint> effects;

    public ApplyEffectsAction() {
        effects = new List<EffectBlueprint>();
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        foreach (EffectBlueprint effect in effects) effect.ComputeEffectValue(ref actionValue, casterData);
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        List<Effect> appliedEffectList = new List<Effect>();
        foreach (EffectBlueprint effect in effects) appliedEffectList.Add(effect.InstantiateEffect(activeData));
        target.ApplyEffects(appliedEffectList);
    }

    #if UNITY_EDITOR

    /// <summary>
    /// EDITOR-ONLY: Add an Effect to the effect list in the ScriptableObject;
    /// </summary>
    /// <param name="effect"> Effect to add; </param>
    public void AddEffect(EffectBlueprint effect) {
        effects.Add(effect);
    }

    /// <summary>
    /// EDITOR-ONLY: Remove an Effect from the effect list in the ScriptableObject;
    /// </summary>
    /// <param name="effectIndex"> Index of the effect to remove; </param>
    public void RemoveEffect(int effectIndex) {
        effects.RemoveAt(effectIndex);
    }

    #endif
}