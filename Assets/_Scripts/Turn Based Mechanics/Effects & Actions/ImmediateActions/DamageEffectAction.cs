using System;
using UnityEngine;
/// <summary>
/// Action to deplete the hitpoints of a target, effect version;
/// </summary>
[Serializable]
public class DamageEffectAction : ImmediateAction.EffectOnly {

    /// <summary> Whether the value spread is flat or fluid; </summary>
    [SerializeField] private bool flat = true;
    /// <summary> Amount to deduct from the target's hitpoint pool; </summary>
    [SerializeField] private int damageAmount;
    /// <summary> Array defining the value spread; </summary>
    [SerializeField] private int[] damageSpread;

    public DamageEffectAction() { }

    public DamageEffectAction(int damageAmount) {
        this.damageAmount = damageAmount;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData, int duration) {
        int currDamage = flat ? damageAmount
                      : duration >= damageSpread.Length ? damageSpread[damageSpread.Length - 1]
                                                        : damageSpread[duration];
        int computedDamage = casterData.ComputeDamage(currDamage);
        actionValue.damageOverTime += computedDamage;
    }

    public override void Use(StatIteration activeData, Actor target, int duration) {
        int currDamage = flat ? damageAmount
                              : duration >= damageSpread.Length ? damageSpread[damageSpread.Length - 1]
                                                                : damageSpread[duration];
        int computedDamage = activeData.ComputeDamage(currDamage);
        target.DepleteHitpoints(computedDamage);
    }

    #if UNITY_EDITOR

    private int spreadSize;

    protected override void DrawActionGUI() => CJUtils.FieldUtils.IntSpreadField(ref flat, ref damageAmount, ref damageSpread, ref spreadSize);

    #endif
}