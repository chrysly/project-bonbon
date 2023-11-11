using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class DamagePercentAction : ImmediateAction.SkillOnly
{
    [SerializeField] private int percentDamageAmount;

    public DamagePercentAction() { }

    public DamagePercentAction(int percentDamageAmount) {
        this.percentDamageAmount = percentDamageAmount;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        int computedDamage = (int)(casterData.Actor.Hitpoints*(percentDamageAmount*0.01));
        actionValue.immediateDamage +=computedDamage;

    }

    public override void Use(StatIteration activeData, Actor target) {
        int computedDamage = (int)(activeData.Actor.Hitpoints * (percentDamageAmount * 0.01));
        target.DepleteHitpoints(computedDamage);
    }
    
    #if UNITY_EDITOR

    protected override void DrawActionGUI() {
        percentDamageAmount = UnityEditor.EditorGUILayout.IntField("Percent Damage Amount:", percentDamageAmount);
    }

    #endif
}
