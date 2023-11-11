using System;
using UnityEngine;

/// <summary>
/// Action to change an actor's stamina, skill version;
/// </summary>
[Serializable]
public class StaminaChangeSkillAction : ImmediateAction.SkillOnly {

    /// <summary> Stamina added (+) or deducted (-) from the target's stamina pool; </summary>
    [SerializeField] private int staminaAmount;

    public StaminaChangeSkillAction() { }

    public StaminaChangeSkillAction(int staminaAmount) {
        this.staminaAmount = staminaAmount;
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        if (staminaAmount >= 0) target.RefundStamina(staminaAmount);
        else target.ConsumeStamina(staminaAmount);
    }

    #if UNITY_EDITOR

    protected override void DrawActionGUI() {
        staminaAmount = UnityEditor.EditorGUILayout.IntField("Stamina Amount:", staminaAmount);
    }

    #endif
}