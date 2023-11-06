using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleStateMachine;

public class BattleStateInput : StateInput {

    #region Global Variables
    private TurnOrderHandler turnOrderHandler;
    public List<Actor> ActorList { get; private set; }

    private Actor activeActor;
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

    public event System.Action<List<Actor>> OnTurnChange;
    public void PropagateTurnChange(List<Actor> previewList) => OnTurnChange?.Invoke(previewList);
    #endregion

    #region Managers
    public BonbonFactory BonbonFactory { get; private set; }
    #endregion

    public void Initialize() { }

    public void InsertTurnQueue(List<Actor> actorList) {
        turnOrderHandler = new TurnOrderHandler(actorList);
        ActorList = actorList;
        PropagateTurnChange(turnOrderHandler.GetTurnPreview(6));
        activeActor = turnOrderHandler.Advance();
    }

    public void OpenBonbonFactory(BonbonFactory bonbonFactory) {
        BonbonFactory = bonbonFactory;
        BonbonFactory.OpenFactory(GameManager.Instance.CurrLevel);
    }

    /// <summary> Advances until the next undefeated Actor. Returns to initial Actor if not available.</summary>
    public void AdvanceTurn() {
        do {
            PropagateTurnChange(turnOrderHandler.GetTurnPreview(6));
            activeActor = turnOrderHandler.Advance();
        } while (activeActor.Defeated);
        currentTurn++;
    }

    public Actor ActiveActor() => activeActor;

    public int CurrTurn() {
        return currentTurn;
    }
}
