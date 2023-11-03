using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class
    BattleStateMachine : StateMachine<BattleStateMachine, BattleStateMachine.BattleState, BattleStateInput> {

    #region SerializeFields
    [SerializeField] private BonbonFactory bonbonFactory;
    [SerializeField] public BattleUIStateMachine uiStateMachine;
    [SerializeField] private EventSequencer _eventSequencer;
    [SerializeField] private float enemyTurnDuration;   //replace with enemy skill duration
    [SerializeField] private List<Actor> actorList;
    #endregion SerializeFields

    #region Events

    public new delegate void StateTransition(BattleState state, BattleStateInput input);
    public event StateTransition OnStateTransition;

    #endregion Events

    protected override void SetInitialState() {
        SetState<BattleStart>();
        actorList.Sort();       //sort by lowest speed
        actorList.Reverse();    //highest speed = higher priority
        CurrInput.InsertTurnQueue(actorList);
        CurrInput.OpenBonbonFactory(bonbonFactory);
    }

    protected override void Start() {
        base.Start();
        _eventSequencer.OnEventTerminate += ContinueBattle;
        StartCoroutine(StartGame(1.75f));
    }

    protected override void Init() {
        base.Init();
    }

    protected void OnEnable() {
    }

    protected void OnDisable() { }
    
    protected override void Update() {
        base.Update();
    }
    
    protected override void FixedUpdate() {
        base.FixedUpdate();
    }

    #region State Handlers

    public void StartBattle() {
        // Checks whether to progress to Win/Lose state
        CurrInput.ResetSkill();
        bool allEnemiesDead = actorList.All(actor => !(actor is EnemyActor) || actor.Defeated);
        bool allCharactersDead = actorList.All(actor => !(actor is CharacterActor) || actor.Defeated);

        if (allEnemiesDead) {
            CurrState.TriggerBattleWin();
        } else if (allCharactersDead) {
            CurrState.TriggerBattleLose();
        } else {
            CurrState.EnterBattle();
        }
    }

    public void StartBattle(float delay) {
        StartCoroutine(ScheduleNextTurn(delay));
    }

    /// <summary>
    /// Continuation method when BSM is frozen on animate state
    /// </summary>
    public void ContinueBattle() {
        ToggleMachine(false);
        uiStateMachine.ToggleMachine(false);
        if (CurrState is AnimateState) {
            StartBattle(0.3f);
        } else StartBattle();
    }

    public void SkipEnemySelection() {
        StartCoroutine(SkipEnemyAction(enemyTurnDuration));
    }

    public void SwitchToTargetSelect(SkillAction skill) {
        if (CurrState is TurnState) {
            Transition<TargetSelectState>();
            CurrInput.UpdateSkill(skill, null);
        }
    }

    public void ConfirmTargetSelect(Actor actor) {
        CurrInput.UpdateSkill(null, new Actor[] { actor });
        Transition<AnimateState>();
    }

    public void AugmentSkill(BonbonObject bonbon) {
        CurrInput.UpdateSkill(null, null, bonbon);
    }

    public void SwitchToBonbonState(BonbonBlueprint bonbon, int slot, bool[] mask) {
        if (CurrState is TurnState || CurrState is BonbonState) {
            BonbonObject bonbonObject = CurrInput.BonbonFactory.CreateBonbon(bonbon, CurrInput.ActiveActor(), mask);
            for (int i = 0; i < 4; i++) {
                BonbonObject bObject = CurrInput.ActiveActor().BonbonInventory[i];
                if (bObject == null) {
                    Debug.Log("NULL at " + i);
                }
                else {
                    Debug.Log("Bonbon: " + bObject.Name + " at " + i);
                }
            }
            CurrInput.ActiveActor().InsertBonbon(slot, bonbonObject);
            Debug.Log("Bonbon: " + CurrInput.ActiveActor().BonbonInventory[slot]);
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

    // idk if this is ok
    public List<Actor> GetActors() => CurrInput.TurnQueue;

    private IEnumerator StartGame(float duration) {
        yield return new WaitForSeconds(duration);
        StartBattle();
    }
}
