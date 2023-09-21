using System;
using UnityEngine;
/// <summary>
/// Action to replenish the hitpoints of a target;
/// </summary>
[Serializable]
public class HealAction : ImmediateAction {

    /// <summary> Amount to replenish in the target's hitpoint pool; </summary>
    [SerializeField] private int healAmount;

    public HealAction(int healAmount) {
        this.healAmount = healAmount;
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
    }

    #endif
}
