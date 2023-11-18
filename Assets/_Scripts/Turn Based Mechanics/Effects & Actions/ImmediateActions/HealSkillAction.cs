using System;
using UnityEngine;

/// <summary>
/// Action to replenish the hitpoints of a target, skill version;
/// </summary>
[Serializable]
public class HealSkillAction : ImmediateAction.SkillOnly {

    /// <summary> Amount to replenish in the target's hitpoint pool; </summary>
    [SerializeField] private int healAmount;

    public HealSkillAction() { }

    public HealSkillAction(int healAmount) {
        this.healAmount = healAmount;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        int computedHeal = casterData.ComputeHeal(healAmount);
        actionValue.immediateHeal += computedHeal;
    }

    public override void Use(StatIteration activeData, Actor target) {
        int computedHeal = activeData.ComputeHeal(healAmount);
        target.RestoreHitpoints(computedHeal);
    }

    #if UNITY_EDITOR

    protected override void DrawActionGUI() {
        healAmount = UnityEditor.EditorGUILayout.IntField("Heal Amount:", healAmount);
    }

    #endif
}