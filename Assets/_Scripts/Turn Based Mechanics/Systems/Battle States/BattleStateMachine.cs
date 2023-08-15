using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class
    BattleStateMachine : StateMachine<BattleStateMachine, BattleStateMachine.BattleState, BattleStateInput> {

    #region SerializeFields
    [SerializeField] private float battleStartAnimationDuration;
    [SerializeField] private List<Actor> actorList;
    #endregion SerializeFields
    
    protected override void SetInitialState() {
        SetState<BattleStart>();
    }

    protected override void Init() {
        base.Init();
    }

    protected void OnEnable() { }

    protected void OnDisable() {
        
    }
    
    protected override void Update() {
        base.Update();
    }

    public void StartBattle() {
        CurrState.EnterBattle();
    }

    public void StartBattle(float delay) {
        StartCoroutine(ScheduleNextTurn(delay));
    }

    public void AnimateTurn() {
        CurrState.AnimateTurn();
    }

    public IEnumerator ScheduleNextTurn(float delay) {
        yield return new WaitForSeconds(delay);
        StartBattle();
        yield return null;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
    }
}
