using UnityEngine;

public class LifestealAction : ImmediateAction.Generic {

    /// <summary> Amount to deduct from the target's hitpoint pool; </summary>
    [SerializeField] private int damageAmount;
    /// <summary> Fraction of damage to replenish in the caster's hitpoint pool (0-100%); </summary>
    [SerializeField] private int lifestealPercent;
    private int lifestealAmount;

    public LifestealAction() { }

    public LifestealAction(int damageAmount, int lifestealPercent) {
        this.damageAmount = damageAmount;
        this.lifestealPercent = lifestealPercent;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        // damage
        int computedDamage = casterData.ComputePotency(damageAmount);
        actionValue.immediateDamage += computedDamage;

        // life steal
        lifestealAmount = (int)(computedDamage * lifestealPercent / 100);
        int computedHeal = casterData.ComputePotency(lifestealAmount);
        actionValue.immediateHeal += computedHeal;
    }

    public override void Use(StatIteration activeData, Actor target) {
        // damage
        int computedDamage = activeData.ComputePotency(damageAmount);
        target.DepleteHitpoints(computedDamage);

        // lifesteal
        lifestealAmount = (int)(computedDamage * lifestealPercent / 100);
        int computedHeal = activeData.ComputePotency(lifestealAmount);
        activeData.Actor.RestoreHitpoints(computedHeal);
    }

#if UNITY_EDITOR

    protected override void DrawActionGUI() {
        damageAmount = UnityEditor.EditorGUILayout.IntField("Damage Amount:", damageAmount);
        lifestealPercent = UnityEditor.EditorGUILayout.IntField("Life-Steal Percent:", lifestealPercent);
    }

#endif
}
