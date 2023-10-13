using UnityEngine;
using UnityEditor;

/// <summary>
/// Scriptable Object class to store bonbon definitions;
/// </summary>
[CreateAssetMenu(menuName = "Bonbon/Bonbon")]
public class BonbonBlueprint : ScriptableObject {

    public new string name;
    public Texture texture;

    /// <summary> A description of the bonbon for later use in the UI; </summary>
    public readonly string description;

    public int craftStamina;

    /// <summary> An bundle of modifiers granted by the bonbon; </summary>
    public PassiveModifier passiveModifiers;

    /// <summary> A bundle of modifiers and actions for bonbon usage; </summary>
    public SkillAugment augmentData;

    /// <summary> A dictionary containing the bonbons required to make the bonbon, mapped to the required quantity; </summary>
    public BonbonBlueprint[] recipe;

    public BonbonBlueprint() {
        recipe = new BonbonBlueprint[4];
    }

    public BonbonObject InstantiateBonbon(Actor actor) {
        actor.ConsumeStamina(craftStamina);
        return new BonbonObject(this);
    }

    public override bool Equals(object other) {
        if (other is BonbonBlueprint) {
            BonbonBlueprint oBonbon = other as BonbonBlueprint;
            return name == oBonbon.name //&& passiveModifiers.Equals(effects, oBonbon.passiveModifiers)
                           && recipe.RecipeEquals(oBonbon.recipe);
        } return false;
    }

    #if UNITY_EDITOR

    public void AddRecipeSlot(BonbonBlueprint replacement) {
        UpdateRecipeSize();
        for (int i = 0; i < recipe.Length; i++) {
            if (recipe[i] == null) {
                recipe[i] = replacement;
                return;
            }
        } Debug.LogWarning("There's no space to place the ingredient;");
    }

    public void RemoveRecipeSlot(BonbonBlueprint removal) {
        UpdateRecipeSize();
        for (int i = 0; i < recipe.Length; i++) {
            if (recipe[i] == removal) {
                recipe[i] = null;
                return;
            }
        } Debug.LogWarning("The Bonbon was not present in the recipe;");
    }

    public void UpdateRecipeSize() {
        if (recipe == null) recipe = new BonbonBlueprint[4];
        else if (recipe.Length < 4) {
            BonbonBlueprint[] nRecipe = new BonbonBlueprint[4];
            for (int i = 0; i < recipe.Length; i++) nRecipe[i] = recipe[i];
            recipe = nRecipe;
        }
    }

    public static GUIContent GUIContent(object bonbonBlueprint) {
        BonbonBlueprint bp = bonbonBlueprint as BonbonBlueprint;
        if (bp.texture == null) return new GUIContent(bp.name);
        else return new GUIContent(bp.texture);
    }

    #endif

    public override int GetHashCode() {
        return System.HashCode.Combine(base.GetHashCode(), name, recipe);
    }
}