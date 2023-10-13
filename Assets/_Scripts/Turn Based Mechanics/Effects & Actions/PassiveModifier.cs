using System;
/// <summary>
/// Bundle class for stat modifiers;
/// <br></br> Belongs in Actors and Bonbons;
/// </summary>
[Serializable]
public class PassiveModifier {

    /// <summary> </summary>
    public int flatAttack;
    /// <summary> Multiplier for the Actor attack attribute; </summary>
    public float percentAttack;
    /// <summary> </summary>
    public int flatHeal;
    /// <summary> Multiplier for the Actor heal cast attribute; </summary>
    public float percentHealCast;
    /// <summary> </summary>
    public int flatDefense;
    /// <summary> Multiplier for the Actor attack attribute; </summary>
    public float percentDefense;
    /// <summary> </summary>
    public int flatSpeed;
    /// <summary> </summary>
    public float percentSpeed;
    /// <summary> Multiplier for the Actor stamina regen attribute; </summary>
    public float percentStaminaRegen;
}
