using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to handle the creation and combination of bonbon objects;
/// </summary>
public class BonbonFactory : MonoBehaviour {

    [SerializeField] private BonbonMap bonbonMapSO;
    /// <summary> An array referencing all bonbons used in battle; </summary>
    private List<BonbonBlueprint> allBonbons;

    /// <summary>
    /// Initialize the Bonbon Factory;
    /// </summary>
    /// <param name="lvlIndex"> Index of the current level; </param>
    public void OpenFactory(int lvlIndex) {
        for (int i = 0; i < lvlIndex; i++) {
            foreach (BonbonBlueprint bonbon in bonbonMapSO.bonbonMap[i]) allBonbons.Add(bonbon);
        }
    }

    /// <summary>
    /// Create a bonbon with a given name using an assortment of bonbon objects;
    /// </summary>
    /// <param name="name"> Name of the bonbon to create (a bonbon object with this name MUST exist!); </param>
    /// <param name="recipeBonbons"> Bonbons used to create the new bonbon; </param>
    /// <returns> A bonbon object if the recipe is valid, NULL otherwise;
    /// <br></br> Note: This method will throw an Exception if the string is invalid! </returns>
    public BonbonObject CreateBonbon(string name, ref BonbonObject[] bonbonInventory, bool[] recipeMask) {
        FindBonbon(name, out BonbonBlueprint bonbonBlueprint);
        if (bonbonBlueprint != null) {
            return CreateBonbon(bonbonBlueprint, ref bonbonInventory, recipeMask);
        } else throw new System.Exception($"InvalidString: No bonbon named \"{name}\" was found;");
    }

    /// <summary>
    /// Create an instance of a given bonbon object using an assortment of bonbon objects;
    /// </summary>
    /// <param name="bonbon"> A bonbon object to duplicate; </param>
    /// <param name="recipeBonbons"> Bonbons used to create the new bonbon; </param>
    /// <returns> A bonbon object if the recipe is valid, NULL otherwise; </returns>
    public BonbonObject CreateBonbon(BonbonBlueprint bonbon, ref BonbonObject[] bonbonInventory, bool[] recipeMask) {
        BonbonBlueprint[] recipeBonbons = CraftRecipeFromMask(bonbonInventory, recipeMask);
        if (bonbon.recipe.Equals(recipeBonbons)) {
            DestroyUsedIngredients(ref bonbonInventory, recipeMask);
            return bonbon.InstantiateBonbon();
        } else return null;
    }

    private BonbonBlueprint[] CraftRecipeFromMask(BonbonObject[] bonbonInventory, bool[] recipeMask) {
        if (bonbonInventory.Length != recipeMask.Length) throw new System.Exception("Mask length does not match Recipe;");
        List<BonbonBlueprint> maskedList = new List<BonbonBlueprint>();
        for (int i = 0; i < bonbonInventory.Length; i++) {
            if (recipeMask[i]) maskedList.Add(bonbonInventory[i].Data);
        } return maskedList.ToArray();
    }

    /// <summary>
    /// Destroy the ingredients spent when creating a bonbon object;
    /// </summary>
    private void DestroyUsedIngredients(ref BonbonObject[] bonbonInventory, bool[] recipeMask) {
        for (int i = 0; i < bonbonInventory.Length; i++) {
            if (recipeMask[i]) bonbonInventory[i] = null;
        }
    }

    /// <summary>
    /// Check whether a bonbon exists with a given name;
    /// </summary>
    /// <param name="name"> Name of the bonbon to search; </param>
    /// <param name="bonbonRes"> Is assigned a bonbon with a matching name, or NULL if none is found; </returns>
    private void FindBonbon(string name, out BonbonBlueprint bonbonRes) {
        foreach (BonbonBlueprint bonbonObj in allBonbons) {
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
    public List<BonbonBlueprint> FindRecipes(params BonbonBlueprint[] recipeBonbons) {
        if (recipeBonbons.Length == 0) return null;
        List<BonbonBlueprint> matchingRecipes = new List<BonbonBlueprint>();
        foreach (BonbonBlueprint bonbonObject in allBonbons) {
            if (bonbonObject.recipe.Length > 0
                && bonbonObject.recipe.RecipeEquals(recipeBonbons)) matchingRecipes.Add(bonbonObject);
        } return matchingRecipes.Count > 0 ? matchingRecipes : null;
    }
}