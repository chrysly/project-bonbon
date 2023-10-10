using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleUIStateMachine : StateMachine<BattleUIStateMachine, BattleUIStateMachine.BattleUIState, BattleUIStateInput> {
    [SerializeField] private BattleStateMachine _battleStateMachine;

    protected override void Start() {
        base.Start();
        _battleStateMachine.OnStateTransition += Refresh;
    }

    protected override void SetInitialState() {
        SetState<InitUIState>();
    }

    protected void Refresh(BattleStateMachine.BattleState state, BattleStateInput input) {
        if (state is BattleStateMachine.TurnState) {
            SetState<InitUIState>();
        }
    }

    protected void LockUI() {
        CurrInput.Locked = true;
    }

    protected void UnlockUI() {
        CurrInput.Locked = false;
    }
    

    protected override void Init() {
        base.Init();
    }
    
    protected void OnEnable() { }

    protected void OnDisable() { }
    
    protected override void Update() {
        base.Update();
    }
    
    protected override void FixedUpdate() {
        base.FixedUpdate();
    }
}
