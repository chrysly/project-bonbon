using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public Actor[] targets;
    } public ActiveSkillPrep SkillPrep { get; private set; }
    #endregion Turn Variables

    // event sequencer tests (DEL LATER)
    public EventSequencer eventSequencer = GameObject.FindAnyObjectByType<EventSequencer>();


    public void Initialize() => SkillPrep = new ActiveSkillPrep();

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

    public void SetSkillPrep(SkillAction skillAction, Actor[] targets) {
        SkillPrep.skill = skillAction;
        SkillPrep.targets = targets;
    }

    public void ActivateSkill() {
        if (SkillPrep.targets.Length > 0) {
            SkillPrep.skill.ActivateSkill(SkillPrep.targets);
        }
        SkillPrep = new ActiveSkillPrep();
    }

    #endregion

    /// <summary> Advances until the next undefeated Actor. Returns to initial Actor if not available.</summary>
    public void AdvanceTurn() 
    {
        Debug.Log("test");
        // some event sequencer tests (DELETE LATER)        // i think the seq check should be here bc it checks stuff at the end of a turn. not sure how to check how to start a seq rn other than a fkcing long switch statemtn or smthing lol
        if (ActiveActor().Hitpoints == 115)
        {
            Debug.Log("It got here");
            eventSequencer.StartEventSequence();
        }

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
