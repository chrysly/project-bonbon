using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to handle the creation and combination of bonbon objects;
/// </summary>
public class BonbonFactory : MonoBehaviour {

    /// <summary> An array referencing all bonbons used in battle; </summary>
    [SerializeField] private BonbonObject[] allBonbons;

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
    /// Create an instance of a given bonbon object using an assortment of bonbon objects;
    /// </summary>
    /// <param name="bonbon"> A bonbon object to duplicate; </param>
    /// <param name="recipeBonbons"> Bonbons used to create the new bonbon; </param>
    /// <returns> A bonbon object if the recipe is valid, NULL otherwise; </returns>
    public BonbonObject CreateBonbon(BonbonObject bonbon, params BonbonObject[] recipeBonbons) {
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
    /// Return a list of bonbons whose recipes match the passed bonbon parameters;
    /// </summary>
    /// <param name="recipeBonbons"> Bonbons available to create a new one; </param>
    /// <returns> A list of matching recipes, or NULL if no matching recipes were found; </returns>
    public List<BonbonObject> FindRecipes(params BonbonObject[] recipeBonbons) {
        if (recipeBonbons.Length == 0) return null;
        List<BonbonObject> matchingRecipes = new List<BonbonObject>();
        foreach (BonbonObject bonbonObject in allBonbons) {
            if (bonbonObject.recipe.Length > 0
                && bonbonObject.recipe.RecipeEquals(recipeBonbons)) matchingRecipes.Add(bonbonObject);
        } return matchingRecipes.Count > 0 ? matchingRecipes : null;
    }
}