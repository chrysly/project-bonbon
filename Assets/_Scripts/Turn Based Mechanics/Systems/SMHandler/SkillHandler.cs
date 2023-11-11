using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHandler : MonoBehaviour {

    [SerializeField] private BattleStateMachine battleStateMachine;

    public ActiveSkillPrep SkillPrep { get; private set; }

    void Awake() {
        SkillPrep = new ActiveSkillPrep();
    }

    void Start() {
        battleStateMachine.CurrInput.OnSkillUpdate += SkillHandler_OnSkillUpdate;
        battleStateMachine.CurrInput.OnRetrieveSkillPrep += () => SkillPrep;
        battleStateMachine.CurrInput.OnSkillReset += SkillHandler_OnSkillReset;
        battleStateMachine.CurrInput.OnSkillActivate += SkillHandler_OnSkillActivate;
    }
    
    public ActiveSkillPrep SkillHandler_OnSkillUpdate(SkillAction skillAction, Actor[] targets, BonbonObject bonbon = null) {
        if (skillAction != null) SkillPrep.skill = skillAction;
        if (targets != null) SkillPrep.targets = targets;
        if (bonbon != null) SkillPrep.bonbon = bonbon;
        return SkillPrep;
    }

    public void SkillHandler_OnSkillReset() => Reset(); 

    public ActiveSkillPrep SkillHandler_OnSkillActivate() {
        if (SkillPrep.targets.Length > 0) {
            if (SkillPrep.bonbon == null) SkillPrep.skill.ActivateSkill(SkillPrep.targets);
            else SkillPrep.skill.AugmentSkill(SkillPrep.targets, SkillPrep.bonbon);
        } return SkillPrep;
    }

    public void Reset() => SkillPrep = new ActiveSkillPrep();
}