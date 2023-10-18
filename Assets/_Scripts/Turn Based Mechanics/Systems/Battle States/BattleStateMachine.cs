using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class
    BattleStateMachine : StateMachine<BattleStateMachine, BattleStateMachine.BattleState, BattleStateInput> {

    #region SerializeFields
    [SerializeField] private BonbonFactory bonbonFactory;
    [SerializeField] private BattleUIStateMachine _uiStateMachine;
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
    // jasmine's jank asf code whooo
    public void OnStart() {
        // hard coded bc it's fcking 5am fml
        _eventSequencer.StartEvent();
        ToggleMachine(true);
        StartBattle();
    }

    public void StartBattle() {
        // Checks whether to progress to Win/Lose state
        bool allEnemiesDead = true;
        bool allCharactersDead = true;
        foreach (Actor actor in actorList) {
            if (actor.Defeated) {
                continue;
            }
            if (actor is EnemyActor) {
                allEnemiesDead = false;
            } else if (actor is CharacterActor) {
                Debug.Log("am alive");
                allCharactersDead = false;
            }
        }

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
        if (CurrState is AnimateState) {
            ToggleMachine(false);
            StartBattle(0.3f);
        }
        if (CurrState is BattleState) { // fix m2
            ToggleMachine(false);
            StartBattle();
        }
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
    public List<Actor> GetActors()
    {
        return actorList;
    }
}
