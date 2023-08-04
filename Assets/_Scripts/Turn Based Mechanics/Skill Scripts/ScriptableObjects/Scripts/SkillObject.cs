using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType { OFFENSIVE, SUPPORT, OTHER };

public abstract class SkillObject : ScriptableObject {

    [Header("Universal Skill Traits")]
    [Tooltip("The string ID of the skill. See drive for naming conventions.")]
    [SerializeField] private int skillID = 0;

    [Tooltip("The name of the skill. Type how this skill name would appear in game.")]
    [SerializeField] private string skillName = "VANILLA";

    [Tooltip("The category the skill falls under. Offensive by default.")]
    [SerializeField] private SkillType skillType = SkillType.OFFENSIVE;

    [Tooltip("The action point cost of the skill. Should not exceed max action turns.")]
    [SerializeField] private int cost = 1;

    [Header("Display")]
    [Tooltip("Cursor prefab to display when skill is part of active path.")]
    [SerializeField] private GameObject activePrefab;
    [Tooltip("Cursor material to display when skill is part of preview path.")]
    [SerializeField] private Material previewMaterial;

    private CursorType cursor;

    public abstract void InitSkillDisplay(ActionDisplay display);

    public abstract void RunSkill(SkillAction action);

    public int GetSkillID() { return skillID; }
    public string GetSkillName() { return skillName; }
    public SkillType GetSkillType() { return skillType; }
    public CursorType GetCursorType() { return cursor; }

    public GameObject GetActivePrefab() { return activePrefab; }
    public Material GetPreviewPrefab() { return previewMaterial; }

    public int GetCost() { return cost; }
}
