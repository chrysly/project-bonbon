using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BonbonSortingUtils {

    /// <summary>
    /// Compare two recipe arrays for content equality;
    /// </summary>
    /// <param name="recipe1"> The recipe calling the comparison; </param>
    /// <param name="recipe2"> The other recipe participating in the comparison; </param>
    /// <returns> Whether the recipes are equivalent; </returns>
    public static bool RecipeEquals(this BonbonObject[] recipe1, BonbonObject[] recipe2) {
        List<BonbonObject> list1 = new List<BonbonObject>(recipe1);
        List<BonbonObject> list2 = new List<BonbonObject>(recipe2);
        foreach (BonbonObject bonbonObj in list1) {
            if (!list2.Remove(bonbonObj)) return false;
        } return list2.Count == 0;
    }
}
