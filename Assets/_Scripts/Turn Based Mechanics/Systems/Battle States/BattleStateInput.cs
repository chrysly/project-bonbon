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

    // event sequencer tests (DEL LATER)
    public EventSequencer eventSequencer = GameObject.FindAnyObjectByType<EventSequencer>();
    

    public void InsertTurnQueue(List<Actor> queue) {
        turnQueue = queue;
    }

    public void SetActiveSkill(SkillAction skill) {
        activeSkill = skill;
    }

    /// <summary> Advances until the next undefeated Actor. Returns to initial Actor if not available.</summary>
    public void AdvanceTurn() 
    {
        Debug.Log("test");
        // some event sequencer tests (DELETE LATER)        // i think the seq check should be here bc it checks stuff at the end of a turn. not sure how to check how to start a seq rn other than a fkcing long switch statemtn or smthing lol
        if (ActiveActor().Hitpoints() == 115)
        {
            Debug.Log("It got here");
            eventSequencer.StartEventSequence();
        }

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

    public List<Actor> GetTurnQueue()
    {
        return turnQueue;
    }
}
