using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHandler : MonoBehaviour {

    [SerializeField] private BattleStateMachine bsm;

    public class ActiveSkillPrep {
        public SkillAction skill;
        public BonbonObject bonbon;
        public Actor[] targets;
    } public ActiveSkillPrep SkillPrep { get; private set; }

    public event System.Action<ActiveSkillPrep> OnSkillTrigger;

    void Awake() {
        SkillPrep = new ActiveSkillPrep();
    }

    void Start() {
        
    }
    
    public void SkillHandler_OnSkillUpdate(SkillAction skillAction, Actor[] targets, BonbonObject bonbon = null) {
        if (skillAction != null) SkillPrep.skill = skillAction;
        if (targets != null) SkillPrep.targets = targets;
        SkillPrep.bonbon = bonbon;
    }

    public void SkillHandler_OnSkillReset() => Reset();

    public void SkillHandler_OnSkillActivate() {
        OnSkillTrigger?.Invoke(SkillPrep);
        ActivateSkill();
    }

    public void ActivateSkill() {
        if (SkillPrep.targets.Length > 0) {
            if (SkillPrep.bonbon == null) SkillPrep.skill.ActivateSkill(SkillPrep.targets);
            else SkillPrep.skill.AugmentSkill(SkillPrep.targets, SkillPrep.bonbon);
        }
    }

    public void Reset() => SkillPrep = new ActiveSkillPrep();
}
