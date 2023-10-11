using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BonbonSortingUtils {

    public static void ConsumeBonbon(this BonbonObject[] inventory, Actor actor, int index) {
        SkillAugment augment = inventory[index].AugmentData;
        /// Apply Bonbon Effect to Caster;
        new ApplyEffectsAction(new List<EffectBlueprint>(new[] { augment.bonbonEffect })).Use(actor.ActiveData, actor);
        /// Trigger a series of immediate actions on the augment;
        foreach (ImmediateAction action in augment.immediateActions) action.Use(actor.ActiveData, actor);
    }

    /// <summary>
    /// Compare two recipe arrays for content equality;
    /// </summary>
    /// <param name="recipe1"> The recipe calling the comparison; </param>
    /// <param name="recipe2"> The other recipe participating in the comparison; </param>
    /// <returns> Whether the recipes are equivalent; </returns>
    public static bool RecipeEquals(this BonbonBlueprint[] recipe1, BonbonBlueprint[] recipe2) {
        if (recipe1 == null || recipe2 == null) return true;
        List<BonbonBlueprint> list1 = new List<BonbonBlueprint>(recipe1);
        List<BonbonBlueprint> list2 = new List<BonbonBlueprint>(recipe2);
        foreach (BonbonBlueprint bonbonObj in list1) {
            if (!list2.Remove(bonbonObj)) return false;
        } return list2.Count == 0;
    }
}
