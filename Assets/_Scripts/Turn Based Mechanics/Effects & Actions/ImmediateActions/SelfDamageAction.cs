using System;
using UnityEngine;
/// <summary>
/// Action to deplete the hitpoints of the caster without target selection;
/// </summary>
[Serializable]
public class SelfDamageAction : ImmediateAction {

    /// <summary> Amount to deduct from the caster's hitpoint pool; </summary>
    [SerializeField] private int damageAmount;

    public SelfDamageAction(int damageAmount) {
        this.damageAmount = damageAmount;
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
    }

    #endif
}
