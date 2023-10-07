using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class
    BattleStateMachine : StateMachine<BattleStateMachine, BattleStateMachine.BattleState, BattleStateInput> {

    #region SerializeFields
    [SerializeField] private BonbonFactory bonbonFactory;
    [SerializeField] private float battleStartAnimationDuration;
    [SerializeField] private float enemyTurnDuration;   //replace with enemy skill duration
    [SerializeField] private List<Actor> actorList;
    #endregion SerializeFields
    
    #region Events
    public new delegate void StateTransition(BattleState state, BattleStateInput input);
    public event StateTransition OnStateTransition ;
    #endregion Events
    
    protected override void SetInitialState() {
        SetState<BattleStart>();
        actorList.Sort();       //sort by lowest speed
        actorList.Reverse();    //highest speed = higher priority
        CurrInput.Initialize();
        CurrInput.InsertTurnQueue(actorList);
        CurrInput.OpenBonbonFactory(bonbonFactory);
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

    public void SkipEnemySelection() {
        StartCoroutine(SkipEnemyAction(enemyTurnDuration));
    }

    public void SwitchToTargetSelect(SkillAction skill) {
        if (CurrState is TurnState) {
            Transition<TargetSelectState>();
            CurrInput.SetSkillPrep(skill);
        }
    }

    public void SwitchToBonbonState(BonbonBlueprint bonbon, int slot, bool[] mask) {
        if (CurrState is TurnState) {
            BonbonObject bonbonObject = CurrInput.BonbonFactory.CreateBonbon(bonbon, CurrInput.ActiveActor().BonbonInventory, mask);
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
            Transition<BonbonState>();
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
