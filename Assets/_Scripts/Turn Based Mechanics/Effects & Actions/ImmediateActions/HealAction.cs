using System;
using UnityEngine;
/// <summary>
/// Action to replenish the hitpoints of a target;
/// </summary>
[Serializable]
public class HealAction : ImmediateAction.Generic {

    /// <summary> Amount to replenish in the target's hitpoint pool; </summary>
    [SerializeField] private int healAmount;

    public HealAction() { }

    public HealAction(int healAmount) {
        this.healAmount = healAmount;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        int computedHeal = casterData.ComputePotency(healAmount);
        actionValue.immediateHeal += computedHeal;
    }

    public override void Use(StatIteration activeData, Actor target, SkillAugment augment) {
        int computedHeal = activeData.ComputePotency(healAmount) + (augment != null ? augment.healBoost : 0); ;
        target.RestoreHitpoints(computedHeal);
    }

    #if UNITY_EDITOR

    protected override void DrawActionGUI() {
        healAmount = UnityEditor.EditorGUILayout.IntField("Heal Amount:", healAmount);
    }

    #endif
}
