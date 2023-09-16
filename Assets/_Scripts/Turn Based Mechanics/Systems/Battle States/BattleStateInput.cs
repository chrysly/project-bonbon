using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateInput : StateInput {

    #region Global Variables
    private List<Actor> turnQueue;
    private int currActorIndex = 0;
    private int currentTurn = 0;
    #endregion Global Variables
    
    #region Turn Variables

    private SkillAction activeSkill;
    #endregion Turn Variables
    

    public void InsertTurnQueue(List<Actor> queue) {
        turnQueue = queue;
    }

    public void SetActiveSkill(SkillAction skill) {
        activeSkill = skill;
    }

    //Advances until the next undefeated Actor. Returns to initial Actor if not available.
    public void AdvanceTurn() {
        Actor initialActor = ActiveActor();
        do {
            currActorIndex = (currActorIndex + 1) % turnQueue.Count;
        } while (ActiveActor().Defeated() && !initialActor.Equals(ActiveActor()));
        currentTurn++;
    }

    public Actor ActiveActor() {
        return turnQueue[currActorIndex];
    }

    public SkillAction ActiveSkill() {
        return activeSkill;
    }

    public int CurrTurn() {
        return currentTurn;
    }
}
