using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorData : ScriptableObject
{
    [Header("Actor Details")]
    
    [Tooltip("As displayed in game.")]
    [SerializeField] private string displayName = "Dummy";
    
    [Tooltip("See programming guidelines for ID conventions.")]
    [SerializeField] private string id;
    
    [Header("Character Attributes")]
    
    [SerializeField] private int maxHitpoints = 300;

    [Tooltip("Base attack power.")]
    [SerializeField] private int baseAttack = 10;
    
    [Tooltip("Base defense.")]
    [SerializeField] private int baseDefense = 10;

    [Tooltip("Base speed. Affects the stack order in battle.")]
    [SerializeField] private int baseSpeed = 10;
    
    [Tooltip("The maximum amount of stamina the character can have.")]
    [SerializeField] private int maxStamina = 100;
    
    [Tooltip("The amount of stamina the character replenishes per turn.")]
    [SerializeField] private int staminaRegenRate = 10;

    public string DisplayName() { return displayName; }
    public string ID() { return id; }
    public int MaxHitpoints() { return maxHitpoints; }
    public int BaseAttack() { return baseAttack; }
    public int BaseDefense() { return baseDefense; }
    public int BaseSpeed() { return baseSpeed; }
    public int MaxStamina() { return maxStamina; }
    public int StaminaRegenRate() { return staminaRegenRate; }
}
