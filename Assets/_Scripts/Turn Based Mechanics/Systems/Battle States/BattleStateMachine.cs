using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public partial class
    BattleStateMachine : StateMachine<BattleStateMachine, BattleStateMachine.BattleState, BattleStateInput> {

    #region SerializeFields
    [SerializeField] private float battleStartAnimationDuration;
    [SerializeField] private float enemyTurnDuration;   //replace with enemy skill duration
    [SerializeField] private List<Actor> actorList;
    #endregion SerializeFields
    
    #region Events
    public delegate void StateTransition(BattleState state, BattleStateInput input);
    public event StateTransition OnStateTransition;
    #endregion Events
    
    protected override void SetInitialState() {
        SetState<BattleStart>();
        actorList.Sort();       //sort by lowest speed
        actorList.Reverse();    //highest speed = higher priority
        CurrInput.InsertTurnQueue(actorList);
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

    #region State Handlers
    public void StartBattle() {
        CurrState.EnterBattle();
    }

    public void StartBattle(float delay) {
        StartCoroutine(ScheduleNextTurn(delay));
    }

    public void SkipEnemySelection() {
        StartCoroutine(SkipEnemyAction(enemyTurnDuration));
    }

    public void SwitchToTargetSelect() {
        if (CurrState is TurnState) {
            Transition<TargetSelectState>();
        }
    }

    public void AnimateTurn() {
        CurrState.AnimateTurn();
    }

    private IEnumerator ScheduleNextTurn(float delay) {
        yield return new WaitForSeconds(delay);
        CurrInput.AdvanceTurn();
        StartBattle();
        yield return null;
    }

    private IEnumerator SkipEnemyAction(float delay) {
        yield return new WaitForSeconds(delay);
        AnimateTurn();
        yield return null;
    }
    #endregion
}
