using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An instantiable Bonbon Object class;
/// </summary>
public class BonbonObject {

    public BonbonBlueprint Data { get; private set; }

    public string Name => Data.name;
    public Texture Texture => Data.texture;
    public string Description => Data.description;
    public PassiveModifier PassiveModifiers => Data.passiveModifiers;
    public BonbonBlueprint[] Recipe => Data.recipe;

    public BonbonObject(BonbonBlueprint blueprint) => Data = blueprint;

    public override bool Equals(object other) {
        if (other == null) return false;
        if (other is BonbonObject) {
            BonbonObject oBonbon = other as BonbonObject;
            return Data == oBonbon.Data;
        } return false;
    }

    public override int GetHashCode() {
        return System.HashCode.Combine(base.GetHashCode(), Data.name, Data.recipe);
    }
}