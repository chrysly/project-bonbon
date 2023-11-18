using System;
using UnityEngine;
/// <summary>
/// Action to replenish the hitpoints of a target, effect version;
/// </summary>
[Serializable]
public class HealEffectAction : ImmediateAction.EffectOnly {

    /// <summary> Whether the value spread is flat or fluid; </summary>
    [SerializeField] private bool flat = true;
    /// <summary> Amount to replenish in the target's hitpoint pool; </summary>
    [SerializeField] private int healAmount;
    /// <summary> Array defining the value spread; </summary>
    [SerializeField] private int[] healSpread;

    public HealEffectAction() { }

    public HealEffectAction(int healAmount) {
        this.healAmount = healAmount;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData, int duration) {
        int currHeal = flat ? healAmount
                            : duration >= healSpread.Length ? healSpread[healSpread.Length - 1]
                                                            : healSpread[duration];
        int computedHeal = casterData.ComputeHeal(currHeal);
        actionValue.healOverTime += computedHeal;
    }

    public override void Use(StatIteration activeData, Actor target, int duration) {
        int currHeal = flat ? healAmount
                            : duration >= healSpread.Length ? healSpread[healSpread.Length - 1]
                                                            : healSpread[duration];
        int computedHeal = activeData.ComputeHeal(currHeal);
        target.RestoreHitpoints(computedHeal);
    }

    #if UNITY_EDITOR

    private int spreadSize;

    protected override void DrawActionGUI() => CJUtils.FieldUtils.IntSpreadField(ref flat, ref healAmount, ref healSpread, ref spreadSize);

    #endif
}