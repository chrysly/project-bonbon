using System;
using System.Collections.Generic;
using UnityEngine;

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