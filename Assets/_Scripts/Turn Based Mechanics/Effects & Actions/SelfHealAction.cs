using System;
using UnityEngine;
/// <summary>
/// Action to replenish the hitpoints of the caster without target selection;
/// </summary>
[Serializable]
public class SelfHealAction : ImmediateAction {

    /// <summary> Amount to replenish in the caster's hitpoint pool; </summary>
    [SerializeField] private int healAmount;

    public SelfHealAction(int healAmount) {
        this.healAmount = healAmount;
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
    }

    #endif
}
