using System;
using UnityEngine;
/// <summary>
/// Action to deplete the hitpoints of the caster without target selection;
/// </summary>
[Serializable]
public class SelfDamageAction : ImmediateAction.SkillOnly {

    /// <summary> Amount to deduct from the caster's hitpoint pool; </summary>
    [SerializeField] private int damageAmount;

    public SelfDamageAction() { }

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

    protected override void DrawActionGUI() {
        damageAmount = UnityEditor.EditorGUILayout.IntField("Self-Damage Amount:", damageAmount);
    }

    #endif
}
