using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SkillObject : ScriptableObject {

    [Header("Skill Identifiers")]
    [Tooltip("The string ID of the skill. See drive for naming conventions.")]
    [SerializeField] private string skillID = "DEFAULT";

    [Tooltip("The name of the skill. Type how this skill name would appear in game.")]
    [SerializeField] private string skillName = "VANILLA";

    [Header("Skill Attributes")]

    public int staminaCost;

    public List<EffectAction> effectActions;

    public bool aoe = false;

    /// <summary>
    /// Compute the action values of each action carried by the skill;
    /// </summary>
    /// <param name="actionValue"> AI Value bundle passed down for data collection; </param>
    public void ComputeActionValues(ref AIActionValue actionValue) {
        foreach (EffectAction action in effectActions) {
            action.ComputeActionValue(ref actionValue);
        }
    }

    public string GetSkillID() { return skillID; }
    public string GetSkillName() { return skillName; }
}
