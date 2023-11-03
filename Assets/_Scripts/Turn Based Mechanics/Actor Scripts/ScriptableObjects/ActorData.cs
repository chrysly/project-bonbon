using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PseudoDataStructures;

public class ActorData : ScriptableObject
{
    [Header("Actor Details")]
    
    [Tooltip("As displayed in game.")]
    [SerializeField] private string displayName = "Dummy";

    [SerializeField] private Sprite portrait;

    [Tooltip("See programming guidelines for ID conventions.")]
    [SerializeField] private string id;
    
    [Header("Character Attributes")]
    
    [SerializeField] private int maxHitpoints = 300;

    [Tooltip("Base attack power.")]
    [SerializeField] private int basePotency = 10;
    
    [Tooltip("Base defense.")]
    [SerializeField] private int baseDefense = 10;

    [Tooltip("Base speed. Affects the stack order in battle.")]
    [SerializeField] private int baseSpeed = 10;
    
    [Tooltip("The maximum amount of stamina the character can have.")]
    [SerializeField] private int maxStamina = 100;
    
    [Tooltip("The amount of stamina the character replenishes per turn.")]
    [SerializeField] private int staminaRegenRate = 20;

    public ArrayArray<SkillObject> skillMap;
    public ArrayArray<BonbonBlueprint> bonbonMap;

    public string DisplayName => displayName;
    public Sprite Portrait => portrait;
    public string ID => id;
    public int MaxHitpoints => maxHitpoints;
    public int BasePotency => basePotency;
    public int BaseDefense => baseDefense;
    public int BaseSpeed => baseSpeed;
    public int MaxStamina => maxStamina;
    public int StaminaRegenRate => staminaRegenRate;

    #if UNITY_EDITOR

    public void SetDisplayName(string displayName) => this.displayName = displayName;

    public void SetID(string id) => this.id = id;

    public void SetMaxHitpoints(int maxHitpoints) => this.maxHitpoints = maxHitpoints;

    public void SetBasePotency(int basePotency) => this.basePotency = basePotency;

    public void SetBaseDefense(int baseDefense) => this.baseDefense = baseDefense;

    public void SetBaseSpeed(int baseSpeed) => this.baseSpeed = baseSpeed;

    public void SetMaxStamina(int maxStamina) => this.maxStamina = maxStamina;

    public void SetStaminaRegenRate(int staminaRegenRate) => this.staminaRegenRate = staminaRegenRate;

    #endif
}
