using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Scriptable Object class for all and every bonbon;
/// </summary>
[CreateAssetMenu(menuName = "Bonbon/Bonbon")]
public class BonbonObject : ScriptableObject {

    public new string name;
    public Texture texture;

    /// <summary> A description of the bonbon for later use in the UI; </summary>
    public readonly string description;

    /// <summary> An array of status effects granted by the bonbon; </summary>
    public EffectModifier[] effects;

    /// <summary> A dictionary containing the bonbons required to make the bonbon, mapped to the required quantity; </summary>
    public BonbonObject[] recipe;

    public override bool Equals(object other) {
        if (other is BonbonObject) {
            BonbonObject oBonbon = other as BonbonObject;
            return name == oBonbon.name /*&& StatusEffect.Equals(statusEffects, oBonbon.statusEffects)*/
                           && recipe.RecipeEquals(oBonbon.recipe);
        } return false;
    }

#if UNITY_EDITOR

    public void AddRecipeSlot(BonbonObject replacement) {
        UpdateRecipeSize();
        for (int i = 0; i < recipe.Length; i++) {
            if (recipe[i] == null) {
                recipe[i] = replacement;
                EditorUtility.SetDirty(this);
                return;
            }
        } Debug.LogWarning("There's no space to place the ingredient;");
    }

    public void RemoveRecipeSlot(BonbonObject removal) {
        UpdateRecipeSize();
        for (int i = 0; i < recipe.Length; i++) {
            if (recipe[i] == removal) {
                recipe[i] = null;
                EditorUtility.SetDirty(this);
                return;
            }
        } Debug.LogWarning("The Bonbon was not present in the recipe;");
    }

    public void UpdateRecipeSize() {
        if (recipe == null) recipe = new BonbonObject[4];
        else if (recipe.Length < 4) {
            var nRecipe = new BonbonObject[4];
            for (int i = 0; i < recipe.Length; i++) nRecipe[i] = recipe[i];
            recipe = nRecipe;
        }
    }

#endif

    public override int GetHashCode() {
        return HashCode.Combine(base.GetHashCode(), name, recipe);
    }
}
