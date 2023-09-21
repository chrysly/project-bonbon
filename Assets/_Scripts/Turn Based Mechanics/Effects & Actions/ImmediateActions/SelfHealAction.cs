using System;
using UnityEngine;
/// <summary>
/// Action to replenish the hitpoints of the caster without target selection;
/// </summary>
[Serializable]
public class SelfHealAction : ImmediateAction.SkillOnly {

    /// <summary> Amount to replenish in the caster's hitpoint pool; </summary>
    [SerializeField] private int healAmount;

    public SelfHealAction() { }

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

    protected override void DrawActionGUI() {
        healAmount = UnityEditor.EditorGUILayout.IntField("Self-Heal Amount:", healAmount);
    }

    #endif
}
