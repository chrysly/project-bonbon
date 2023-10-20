using System.Linq;
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

    public static bool PassBonbon(this Actor sourceActor, int sourceSlot, Actor targetActor) {
        if (sourceActor.BonbonInventory[sourceSlot] == null) {
            Debug.Log($"Source inventory #{sourceSlot} is empty...");
            return false;
        }
        int targetSlot = -1;
        for (int i = 0; i < targetActor.BonbonInventory.Length; i++) {
            if (targetActor.BonbonInventory[i] == null) {
                targetSlot = i;
                break;
            }
        } if (targetSlot == -1) return false;
        else {
            targetActor.BonbonInventory[targetSlot] = sourceActor.BonbonInventory[sourceSlot];
            sourceActor.BonbonInventory[sourceSlot] = null;
        } return true;
    }

    /// <summary>
    /// Compare two recipe arrays for content equality;
    /// </summary>
    /// <param name="recipe1"> The recipe calling the comparison; </param>
    /// <param name="recipe2"> The other recipe participating in the comparison; </param>
    /// <returns> Whether the recipes are equivalent; </returns>
    public static bool RecipeEquals(this BonbonBlueprint[] recipe1, BonbonBlueprint[] recipe2) {
        if (recipe1 == null || recipe2 == null) return true;
        List<BonbonBlueprint> list1 = new List<BonbonBlueprint>(recipe1).Where(bb => bb != null).ToList();
        List<BonbonBlueprint> list2 = new List<BonbonBlueprint>(recipe2).Where(bb => bb != null).ToList();
        foreach (BonbonBlueprint bonbonObj in list1) {
            if (!list2.Remove(bonbonObj)) return false;
        } return list2.Count == 0;
    }
}
