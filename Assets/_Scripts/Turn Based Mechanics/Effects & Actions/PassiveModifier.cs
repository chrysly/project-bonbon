using System;
/// <summary>
/// Bundle class for stat modifiers;
/// <br></br> Belongs in Actors and Bonbons;
/// </summary>
[Serializable]
public class PassiveModifier {

    /// <summary> Multiplier for the Actor attack attribute; </summary>
    public float attackModifier;
    /// <summary> Multiplier for the Actor heal cast attribute; </summary>
    public float healCastModifier;
    /// <summary> Multiplier for the Actor attack attribute; </summary>
    public float defenseModifier;
    /// <summary> Multiplier for the Actor stamina regen attribute; </summary>
    public float staminaRegenModifier;

    //public float speedModifier;
    //public float evasionModifier;
}
