using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleStateMachine;

public class BattleStateInput : StateInput {

    #region Global Variables
    private TurnOrderHandler turnHandler;
    private Actor currActor;
    public List<Actor> TurnQueue => turnHandler != null ? turnHandler.GetTurnDisplay() : null;
    private int currentTurn = 0;
    #endregion Global Variables

    #region | Events |

    public event System.Func<SkillAction, Actor[], BonbonObject, ActiveSkillPrep> OnSkillUpdate;
    public void UpdateSkill(SkillAction sa, Actor[] targets, BonbonObject bonbon = null) => OnSkillUpdate?.Invoke(sa, targets, bonbon);

    public event System.Func<ActiveSkillPrep> OnRetrieveSkillPrep;
    public ActiveSkillPrep SkillPrep {
        get {
            ActiveSkillPrep skillPrep = OnRetrieveSkillPrep?.Invoke();
            return skillPrep == null ? new ActiveSkillPrep() : skillPrep;
        }
    }

    public event System.Action OnSkillReset;
    public void ResetSkill() => OnSkillReset?.Invoke();

    public event System.Func<ActiveSkillPrep> OnSkillActivate;
    public ActiveSkillPrep ActivateSkill() => OnSkillActivate?.Invoke();

    public event System.Action<SkillAction, BonbonObject> OnSkillAnimation;
    public void AnimateSkill(SkillAction sa, BonbonObject bonbon) => OnSkillAnimation?.Invoke(sa, bonbon);

    #endregion

    #region Managers
    public BonbonFactory BonbonFactory { get; private set; }
    #endregion

    public void Initialize() { }

    public void InsertTurnHandler(TurnOrderHandler toh) {
        turnHandler = toh;
        currActor = turnHandler.GetTurnDisplay()[0];
    }

    public void OpenBonbonFactory(BonbonFactory bonbonFactory) {
        BonbonFactory = bonbonFactory;
        BonbonFactory.OpenFactory(GameManager.Instance.CurrLevel);
    }

    /// <summary> Advances until the next undefeated Actor. Returns to initial Actor if not available.</summary>
    public void AdvanceTurn() {
        turnHandler.Advance();
        currentTurn++;
    }
    /*
        {
        Actor initialActor = ActiveActor();
        do {
            currActorIndex = (currActorIndex + 1) % turnQueue.Count;
        } while (ActiveActor().Defeated && !initialActor.Equals(ActiveActor()));
        currentTurn++;
    }*/

    public Actor ActiveActor() => currActor;

    public int CurrTurn() {
        return currentTurn;
    }
}
