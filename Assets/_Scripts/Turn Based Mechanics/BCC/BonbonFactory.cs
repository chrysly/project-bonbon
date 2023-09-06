using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to handle the creation and combination of bonbon objects;
/// </summary>
public class BonbonFactory : MonoBehaviour {

    /// <summary> An array referencing all bonbons used in battle; </summary>
    private BonbonObject[] allBonbons;

    /// <summary>
    /// Create a bonbon with a given name using an assortment of bonbon objects;
    /// </summary>
    /// <param name="name"> Name of the bonbon to create (a bonbon object with this name MUST exist!); </param>
    /// <param name="recipeBonbons"> Bonbons used to create the new bonbon; </param>
    /// <returns> A bonbon object if the recipe is valid, NULL otherwise;
    /// <br></br> Note: This method will throw an Exception if the string is invalid! </returns>
    public BonbonObject CreateBonbon(string name, params BonbonObject[] recipeBonbons) {
        FindBonbon(name, out BonbonObject bonbonObject);
        if (bonbonObject) {
            return CreateBonbon(bonbonObject, recipeBonbons);
        } else throw new System.Exception($"InvalidString: No bonbon named \"{name}\" was found;");
    }

    /// <summary>
    /// Create a bonbon of a know index using an assortment of bonbon objects;
    /// </summary>
    /// <param name="bonbonIndex"> Index of the bonbon to create; </param>
    /// <param name="recipeBonbons"> Bonbons used to create the new bonbon; </param>
    /// <returns></returns>
    public BonbonObject CreateBonbon(int bonbonIndex, params BonbonObject[] recipeBonbons) {
        if (bonbonIndex >= allBonbons.Length) throw new System.Exception("Invalid Index: Bonbon index out of bounds;");
        return CreateBonbon(allBonbons[bonbonIndex], recipeBonbons);
    }

    /// <summary>
    /// Create an instance of a given bonbon object using an assortment of bonbon objects;
    /// </summary>
    /// <param name="bonbon"> A bonbon object to duplicate; </param>
    /// <param name="recipeBonbons"> Bonbons used to create the new bonbon; </param>
    /// <returns> A bonbon object if the recipe is valid, NULL otherwise; </returns>
    private BonbonObject CreateBonbon(BonbonObject bonbon, params BonbonObject[] recipeBonbons) {
        if (bonbon.recipe.Equals(recipeBonbons)) {
            DestroyUsedIngredients(recipeBonbons);
            return Instantiate(bonbon);
        } else return null;
    }

    /// <summary>
    /// Destroy the ingredients spent when creating a bonbon object;
    /// </summary>
    /// <param name="usedBonbons"> Ingredients to destroy; </param>
    private void DestroyUsedIngredients(BonbonObject[] usedBonbons) {
        for (int i = 0; i < usedBonbons.Length; i++) Destroy(usedBonbons[i]);
    }

    /// <summary>
    /// Check whether a bonbon exists with a given name;
    /// </summary>
    /// <param name="name"> Name of the bonbon to search; </param>
    /// <param name="bonbonRes"> Is assigned a bonbon with a matching name, or NULL if none is found; </returns>
    private void FindBonbon(string name, out BonbonObject bonbonRes) {
        foreach (BonbonObject bonbonObj in allBonbons) {
            if (bonbonObj.name == name) {
                bonbonRes = bonbonObj;
                return;
            }
        } bonbonRes = null;
    }

    /// <summary>
    /// Check whether the passed bonbons match an available recipe;
    /// <br></br> It is assumed that no two complex bonbons will have the same recipe;
    /// </summary> /// It's not hard to return an array of bonbons instead, but will only do it if we need it;
    /// <param name="recipeBonbons"> Bonbons that will be destroyed to create a new bonbon; </param>
    /// <returns> The index of a bonbon that can be made using the passed bonbons, or -1 if none is found; </returns>
    public int FindRecipe(params BonbonObject[] recipeBonbons) {
        if (recipeBonbons.Length == 0) return -1;
        for (int i = 0; i < allBonbons.Length; i++) {
            BonbonObject bonbonObject = allBonbons[i];
            if (bonbonObject.recipe.Length > 0
                && bonbonObject.recipe.RecipeEquals(recipeBonbons)) return i;
        } return -1;
    }
}