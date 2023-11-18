using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleStateMachine;

public class BattleStateInput : StateInput {

    #region Global Variables
    private TurnOrderHandler turnOrderHandler;
    public List<Actor> ActorList => ActorHandler.ActorList;

    private Actor activeActor;
    private int currentTurn = 0;

    public ActiveSkillPrep SkillPrep => SkillHandler.SkillPrep;

    public event System.Action<List<Actor>> OnTurnChange;
    public void PropagateTurnChange(List<Actor> previewList) => OnTurnChange?.Invoke(previewList);

    #endregion Global Variables

    #region Managers
    public ActorHandler ActorHandler { get; private set; }
    public SkillHandler SkillHandler { get; private set; }
    public AnimationHandler AnimationHandler { get; private set; }
    public BonbonHandler BonbonHandler { get; private set; }
    public GlobalCameraManager CameraHandler { get; private set; }
    #endregion

    /// <summary>
    /// Initialize the State Machine Handlers in order;
    /// </summary>
    /// <param name="handlers"></param>
    public void Initialize(StateMachineHandler[] handlers) {
        /// First loop => Grab references;
        foreach (StateMachineHandler smh in handlers) {
            if (smh is ActorHandler) ActorHandler = smh as ActorHandler;
            else if (smh is SkillHandler) SkillHandler = smh as SkillHandler;
            else if (smh is AnimationHandler) AnimationHandler = smh as AnimationHandler;
            else if (smh is BonbonHandler) BonbonHandler = smh as BonbonHandler;
            else if (smh is GlobalCameraManager) CameraHandler = smh as GlobalCameraManager;
        } /// Second loop => Initialize Handlers;
        foreach (StateMachineHandler smh in handlers) smh.Initialize(this);
    }

    public void InitializeTurnOrder(List<Actor> actorList) {
        turnOrderHandler = new TurnOrderHandler(actorList);
        PropagateTurnChange(turnOrderHandler.GetTurnPreview(6));
        activeActor = turnOrderHandler.Advance();
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
