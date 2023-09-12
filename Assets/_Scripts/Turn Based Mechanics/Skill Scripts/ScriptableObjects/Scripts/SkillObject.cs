using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu()]
public class SkillObject : ScriptableObject {

    [Header("Skill Identifiers")]
    [Tooltip("The string ID of the skill. See drive for naming conventions.")]
    [SerializeField] private string skillID = "DEFAULT";

    [Tooltip("The name of the skill. Type how this skill name would appear in game.")]
    [SerializeField] private string skillName = "VANILLA";

    [Header("Skill Attributes")] 
    public float damageAmount = 5f;
    public float healAmount = 0f;
    public float resistanceAmount = 0f;
    public float speedIncreaseAmount = 0f;
    public float attackIncreaseAmount = 0f;
    public float selfInflictAmount = 0f;
    
    public String GetSkillID() { return skillID; }
    public string GetSkillName() { return skillName; }
}
