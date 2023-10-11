/// <summary>
/// A bundle of AI values from skill actions;
/// </summary>
public class AIActionValue {

    /// <summary> Damage triggered immediately upon skill use; </summary>
    public int immediateDamage;
    /// <summary> Heal triggered immediately upon skill use; </summary>
    public int immediateHeal;
    /// <summary> Damage distributed over several turns; </summary>
    public int damageOverTime;
    /// <summary> Heal distributed over several turns; </summary>
    public int healOverTime;
    /// <summary> Special skill priority; </summary>
    public int specialValue;

    public Actor caster;
    public Actor target;
}