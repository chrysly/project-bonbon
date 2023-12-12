using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An instantiable Bonbon Object class;
/// </summary>
public class BonbonObject {

    public BonbonBlueprint Data { get; private set; }

    public string Name => Data.name;
    public Sprite Texture => Data.texture;
    public string Description => Data.description;

    public int CraftStamina => Data.craftStamina;
    public SkillAugment AugmentData => Data.augmentData;
    public BonbonBlueprint[] Recipe => Data.recipe;

    public BonbonObject(BonbonBlueprint blueprint) => Data = blueprint;

    public BonbonDisplayObject displayObject;

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