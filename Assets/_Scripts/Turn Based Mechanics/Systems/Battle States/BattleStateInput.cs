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

    public void AdvanceTurn() {
        if (currActorIndex < turnQueue.Count - 1) {
            currActorIndex++;
        } else {
            currActorIndex = 0;
        }
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
