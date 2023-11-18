using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class
    BattleStateMachine : StateMachine<BattleStateMachine, BattleStateMachine.BattleState, BattleStateInput> {

    #region SerializeFields
    [SerializeField] public BattleUIStateMachine uiStateMachine;
    [SerializeField] private EventSequencer _eventSequencer;
    [SerializeField] private float enemyTurnDuration;   //replace with enemy skill duration
    #endregion SerializeFields

    #region Events

    public new delegate void StateTransition(BattleState state, BattleStateInput input);
    public event StateTransition OnStateTransition;

    #endregion Events

    #region Singleton

    private static BattleStateMachine instance;
    public static BattleStateMachine Instance => instance;

    #endregion

    protected override void Awake() {
        base.Awake();
        if (instance != null) {
            Destroy(gameObject);
        } else instance = this;
    }

    protected override void SetInitialState() {
        SetState<BattleStart>();
        CurrInput.Initialize(GetComponents<StateMachineHandler>());
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
        CurrInput.SkillHandler.SkillReset();
        bool allEnemiesDead = CurrInput.ActorList.All(actor => !(actor is EnemyActor) || actor.Defeated);
        bool allCharactersDead = CurrInput.ActorList.All(actor => !(actor is CharacterActor) || actor.Defeated);

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
            CurrInput.SkillHandler.SkillUpdate(skill);
        }
    }

    public void ConfirmTargetSelect(Actor actor) {
        CurrInput.SkillHandler.SkillUpdate(new Actor[] { actor });
        Transition<AnimateState>();
    }

    public void AugmentSkill(BonbonObject bonbon) {
        CurrInput.SkillHandler.SkillUpdate(bonbon);
    }

    public void SwitchToBonbonState(BonbonBlueprint bonbon, int slot, bool[] mask) {
        if (CurrState is TurnState || CurrState is BonbonState) {
            BonbonObject bonbonObject = CurrInput.BonbonHandler.CreateBonbon(bonbon, CurrInput.ActiveActor(), mask);
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
    public List<Actor> GetActors() => CurrInput.ActorList;
    public List<Actor> FilterActorsBySkill() {
        SkillAction sa = CurrInput.SkillPrep.skill;
        SkillObject.TargetConstraint targetType = sa.SkillData.TargetType;
        switch (targetType) {
            case SkillObject.TargetConstraint.Enemies:
                return CurrInput.ActorList.Where(actor => sa.Caster.GetType() != actor.GetType()).ToList();
            case SkillObject.TargetConstraint.Allies:
                return CurrInput.ActorList.Where(actor => sa.Caster.GetType() == actor.GetType()).ToList();
            default:
                return CurrInput.ActorList;
        }
    }

    private IEnumerator StartGame(float duration) {
        yield return new WaitForSeconds(duration);
        StartBattle();
    }
}
