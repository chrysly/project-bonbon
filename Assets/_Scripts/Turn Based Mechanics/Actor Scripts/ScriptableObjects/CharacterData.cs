using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu()]

public class CharacterData : ScriptableObject {

    [Header("Character Details")]

    [Tooltip("As displayed in game.")]
    [SerializeField] private string displayName = "Dummy";

    [Tooltip("See programming guidelines for ID conventions.")]
    [SerializeField] private string id;

    [Header("Character Attributes")]

    [Tooltip("The number of hitpoints.")]
    [SerializeField] private int maxHitpoints = 300;

    [Tooltip("Base attack power.")]
    [SerializeField] private float baseAttack = 10;

    [Tooltip("Base defense.")]
    [SerializeField] private float baseDefense = 10;

    [Tooltip("Base speed. Affects the stack order in battle.")]
    [SerializeField] private float baseSpeed = 10;

    [Tooltip("The maximum amount of stamina the character can have.")]
    [SerializeField] private int maxStamina = 100;

    [Tooltip("The amount of stamina the character replenishes per turn.")]
    [SerializeField] private int staminaRegenRate = 10;

    [Tooltip("The maximum amount of actions the character can take per turn.")]
    [SerializeField] private int maxActions = 3;

    [Tooltip("Maximum distance the character can move.")]
    [SerializeField] private float maxDistance = 20;

    [Tooltip("Pool of skills that character is able to learn.")]
    [SerializeField] private List<SkillObject> skillList;

    [Tooltip("Pool of ingredients the character has access to")]
    [SerializeField] private List<Ingredient> ingredientList;

    //TODO: List of Recipes/Ingredients

    #region Getters
    public string DisplayName() { return displayName; }
    public string ID() { return id; }
    public int MaxHitpoints() { return maxHitpoints; }
    public float BaseAttack() { return baseAttack; }
    public float BaseDefense() { return baseDefense; }
    public float BaseSpeed() { return baseSpeed; }
    public int MaxStamina() { return maxStamina; }
    public int StaminaRegenRate() { return staminaRegenRate; }
    public int MaxActions() { return maxActions; }
    public float MaxDistance() { return maxDistance; }
    public List<SkillObject> SkillList() { return skillList; }
    public List<Ingredient> IngredientList() { return ingredientList; }
    #endregion Getters
}
