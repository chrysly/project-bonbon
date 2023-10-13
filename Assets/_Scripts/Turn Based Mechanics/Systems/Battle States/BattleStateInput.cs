using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleStateMachine;

public class BattleStateInput : StateInput {

    #region Global Variables
    private List<Actor> turnQueue;
    private int currActorIndex = 0;
    private int currentTurn = 0;
    #endregion Global Variables

    #region Managers
    public BonbonFactory BonbonFactory { get; private set; }
    #endregion

    #region Turn Variables
    public class ActiveSkillPrep {
        public SkillAction skill;
        public BonbonObject bonbon;
        public Actor[] targets;
    } public ActiveSkillPrep SkillPrep { get; private set; }
    #endregion Turn Variables

    public void Initialize()    // short hand (normal Initialize method) => method that has a single line !!
    {
        SkillPrep = new ActiveSkillPrep();
    }

    public void InsertTurnQueue(List<Actor> queue) {
        turnQueue = queue;
    }

    public void OpenBonbonFactory(BonbonFactory bonbonFactory) {
        BonbonFactory = bonbonFactory;
        BonbonFactory.OpenFactory(GameManager.CurrLevel);
    }

    #region Skill Preparation

    public void SetSkillPrep(SkillAction skillAction) {
        SkillPrep.skill = skillAction;
    }

    public void SetSkillPrep(Actor[] targets) {
        SkillPrep.targets = targets;
    }

    public void SetSkillPrep(BonbonObject bonbon) {
        SkillPrep.bonbon = bonbon;
    }

    public void SetSkillPrep(SkillAction skillAction, Actor[] targets) {
        SkillPrep.skill = skillAction;
        SkillPrep.targets = targets;
    }

    public void ActivateSkill() {
        if (SkillPrep.targets.Length > 0) {
            if (SkillPrep.bonbon == null) SkillPrep.skill.ActivateSkill(SkillPrep.targets);
            else SkillPrep.skill.AugmentSkill(SkillPrep.targets, SkillPrep.bonbon);
        }
    }

    public void resetSkillPrep() {
        SkillPrep = new ActiveSkillPrep();
    }

    #endregion

    /// <summary> Advances until the next undefeated Actor. Returns to initial Actor if not available.</summary>
    public void AdvanceTurn() 
    {
        Actor initialActor = ActiveActor();
        do {
            currActorIndex = (currActorIndex + 1) % turnQueue.Count;
        } while (ActiveActor().Defeated && !initialActor.Equals(ActiveActor()));
        currentTurn++;
    }

    public Actor ActiveActor() {
        return turnQueue[currActorIndex];
    }

    public int CurrTurn() {
        return currentTurn;
    }

    public List<Actor> GetTurnQueue()
    {
        return turnQueue;
    }
}
