using System;
using UnityEngine;
/// <summary>
/// Action to change an actor's stamina;
/// <br></br> Note: I could split this into "Deplete" and "Replenish" Actions on demand;
/// </summary>
[Serializable]
public class StaminaChangeAction : ImmediateAction {

    /// <summary> Stamina added (+) or deducted (-) from the target's stamina pool; </summary>
    [SerializeField] private int staminaAmount;

    public StaminaChangeAction(int staminaAmount) {
        this.staminaAmount = staminaAmount;
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        target.RefundStamina(staminaAmount);
    }


    /// <summary>
    /// EDITOR-ONLY: Change the stamina amount in the ScriptableObject;
    /// </summary>
    /// <param name="amount"> New stamina amount; </param>
    public void Modify(int amount) {
        staminaAmount = amount;
    }
}
