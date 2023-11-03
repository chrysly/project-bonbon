using System;
using UnityEngine;

/// <summary>
/// Action to deplete the hitpoints of a target;
/// </summary>
[Serializable]
public class DamageAction : ImmediateAction.Generic {

    /// <summary> Amount to deduct from the target's hitpoint pool; </summary>
    [SerializeField] private int damageAmount;

    public DamageAction() { }

    public DamageAction(int damageAmount) {
        this.damageAmount = damageAmount;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        int computedDamage = casterData.ComputePotency(damageAmount);
        actionValue.immediateDamage += computedDamage;
    }

    public override void Use(StatIteration activeData, Actor target) {
        int computedDamage = activeData.ComputePotency(damageAmount);
        target.DepleteHitpoints(computedDamage);

        // knock off bonbons % chance ---
        int damagePercent = (computedDamage / activeData.Actor.Hitpoints) * 100;
        int bonbonsToRemove = 0;

        if (damagePercent < 25)
        {
            bonbonsToRemove = 0;
        }
        else if (damagePercent < 50)
        {
            bonbonsToRemove = 1;
        }
        else if (damagePercent < 75)
        {
            bonbonsToRemove = 2;
        }
        else if (damagePercent < 100)
        {
            bonbonsToRemove = 3;
        }

        System.Random rand = new System.Random();
        bonbonsToRemove += rand.Next(2);

        if (bonbonsToRemove > target.BonbonList.Count)
            bonbonsToRemove = target.BonbonList.Count;

        for (int i = 0; i < bonbonsToRemove; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, target.BonbonList.Count);
            //target.BonbonInventory[randomIndex] = null; ADD BACK AAAAAA
        }
    }

    #if UNITY_EDITOR

    protected override void DrawActionGUI() {
        damageAmount = UnityEditor.EditorGUILayout.IntField("Damage Amount:", damageAmount);
    }

    #endif
}
