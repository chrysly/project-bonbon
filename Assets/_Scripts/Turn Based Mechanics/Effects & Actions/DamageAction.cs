using System;
using UnityEngine;
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
