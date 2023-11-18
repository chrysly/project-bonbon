using System;
using UnityEngine;

/// <summary>
/// Action to deplete the hitpoints of a target, skill version;
/// </summary>
[Serializable]
public class DamageSkillAction : ImmediateAction.SkillOnly {

    /// <summary> Amount to deduct from the target's hitpoint pool; </summary>
    [SerializeField] private int damageAmount;

    public DamageSkillAction() { }

    public DamageSkillAction(int damageAmount) {
        this.damageAmount = damageAmount;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        int computedDamage = casterData.ComputeDamage(damageAmount);
        actionValue.immediateDamage += computedDamage;
    }

    public override void Use(StatIteration activeData, Actor target) {
        int computedDamage = activeData.ComputeDamage(damageAmount);
        target.DepleteHitpoints(computedDamage);
    }

    #if UNITY_EDITOR

    protected override void DrawActionGUI() {
        damageAmount = UnityEditor.EditorGUILayout.IntField("Damage Amount:", damageAmount);
    }

    #endif
}