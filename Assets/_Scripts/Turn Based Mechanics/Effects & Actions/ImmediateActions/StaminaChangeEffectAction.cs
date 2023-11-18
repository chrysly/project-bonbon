using System;
using UnityEngine;
/// <summary>
/// Action to change an actor's stamina, effect version;
/// </summary>
[Serializable]
public class StaminaChangeEffectAction : ImmediateAction.EffectOnly {

    /// <summary> Whether the value spread is flat or fluid; </summary>
    [SerializeField] private bool flat = true;
    /// <summary> Amount to replenish in the target's hitpoint pool; </summary>
    [SerializeField] private int staminaAmount;
    /// <summary> Array defining the value spread; </summary>
    [SerializeField] private int[] staminaSpread;

    public StaminaChangeEffectAction() { }

    public StaminaChangeEffectAction(int staminaAmount) {
        this.staminaAmount = staminaAmount;
    }

    public override void Use(StatIteration activeData, Actor target, int duration) {
        int currStamina = flat ? staminaAmount
                               : duration >= staminaSpread.Length ? staminaSpread[staminaSpread.Length - 1]
                                                                  : staminaSpread[duration];
        if (currStamina >= 0) target.RefundStamina(currStamina);
        else target.ConsumeStamina(currStamina);
    }

    #if UNITY_EDITOR

    private int spreadSize;

    protected override void DrawActionGUI() => CJUtils.FieldUtils.IntSpreadField(ref flat, ref staminaAmount, ref staminaSpread, ref spreadSize);

    #endif
}