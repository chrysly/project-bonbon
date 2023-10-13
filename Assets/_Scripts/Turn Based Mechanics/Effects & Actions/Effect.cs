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
        if (actions == null) return;
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