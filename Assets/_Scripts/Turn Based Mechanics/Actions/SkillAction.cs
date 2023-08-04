using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAction : ActorAction
{
    [SerializeField] private SkillObject skill;
    private Vector3 location;
    Transform actor;

    public SkillAction(SkillObject skill, Transform actor) {
        this.skill = skill;
        this.actor = actor;
    }

    public void RunAction(Transform actor, float duration) {
        //TODO: RunAction will likely pull data from a ScriptableObject
        skill.RunSkill(this);
    }

    public void StoreLocation(Vector3 location) {
        this.location = location;
        this.location.y += 1;
    }

    public SkillObject getSkill() {
        return skill;
    }

    public Transform Actor() { return actor; }

    public int GetCost() {
        return skill.GetCost();
    }

    public Vector3 GetLocation() {
        return location;
    }
}
