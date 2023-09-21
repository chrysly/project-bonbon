using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object class for all and every bonbon;
/// </summary>
[CreateAssetMenu(menuName = "Bonbon/Bonbon")]
public class BonbonObject : ScriptableObject {

    public new string name;
    public Sprite sprite;

    /// <summary> A description of the bonbon for later use in the UI; </summary>
    public readonly string description;

    /// <summary> An array of status effects granted by the bonbon; </summary>
    //public StatusEffect[] statusEffects;

    /// <summary> A dictionary containing the bonbons required to make the bonbon, mapped to the required quantity; </summary>
    public BonbonObject[] recipe;

    public BonbonDisplayObject displayObject;

    public override bool Equals(object other) {
        if (other is BonbonObject) {
            BonbonObject oBonbon = other as BonbonObject;
            return name == oBonbon.name /*StatusEffect.Equals(statusEffects, oBonbon.statusEffects)*/
                           && recipe.RecipeEquals(oBonbon.recipe); 
        } return false;
    }

    public override int GetHashCode() {
        return HashCode.Combine(base.GetHashCode(), name, recipe);
    }
}
