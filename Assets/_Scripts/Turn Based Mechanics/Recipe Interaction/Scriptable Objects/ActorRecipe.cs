using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorRecipe : MonoBehaviour
{
    private List<Ingredient> ingredientList;
    private Recipe validRecipe = null;  //Empty if recipe is not valid
    private int recipeTier = 0;

    public void AddIngredient(Ingredient ingredient) {
        recipeTier++;
        ingredientList.Add(ingredient);

        //check if valid recipe, if not, add to ingredient list
    }

    public void InfuseRecipe() {

    }
}
