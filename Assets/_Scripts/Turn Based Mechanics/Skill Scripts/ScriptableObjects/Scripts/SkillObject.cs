using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SkillObject : ScriptableObject {

    [Header("Skill Identifiers")]
    [Tooltip("The string ID of the skill. See drive for naming conventions.")]
    [SerializeField] private string skillID = "DEFAULT";
    public string ID => skillID;

    [Tooltip("The name of the skill. Type how this skill name would appear in game.")]
    [SerializeField] private string skillName = "VANILLA";
    public string Name => skillName;

    [Header("Skill Attributes")]

    public int staminaCost;

    /// <summary> A list of Immediate Actions performed by the skill when used; </summary>
    [HideInInspector] [SerializeReference]
    public List<ImmediateAction> immediateActions;

    public bool aoe = false;

    public void PerformActions(StatIteration casterData, Actor target) {
        foreach (ImmediateAction action in immediateActions) action.Use(casterData, target);
    }

    /// <summary>
    /// Compute the action values of each action carried by the skill;
    /// </summary>
    /// <param name="actionValue"> AI Value bundle passed down for data collection; </param>
    public void ComputeActionValues(ref AIActionValue actionValue, StatIteration casterStats) {
        foreach (ImmediateAction action in immediateActions) {
            action.ComputeActionValue(ref actionValue, casterStats);
        }
    }

    public string GetSkillID() { return skillID; }
    public string GetSkillName() { return skillName; }

    #if UNITY_EDITOR

    public static GUIContent GUIContent(object skillBlueprint) {
        SkillObject bp = skillBlueprint as SkillObject;
        return new GUIContent(bp.skillName);
    }

    #endif
}
