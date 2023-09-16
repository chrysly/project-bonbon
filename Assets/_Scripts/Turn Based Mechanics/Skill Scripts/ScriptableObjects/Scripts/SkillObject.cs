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
    public int damageAmount = 5;
    public int staminaCost = 0;
    public int healAmount = 0;
    public int resistanceAmount = 0;
    public int speedIncreaseAmount = 0;
    public int attackIncreaseAmount = 0;
    public int selfInflictAmount = 0;
    
    public String GetSkillID() { return skillID; }
    public string GetSkillName() { return skillName; }
}
