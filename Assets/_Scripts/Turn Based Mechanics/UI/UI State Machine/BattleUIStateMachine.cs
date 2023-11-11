using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleUIStateMachine : StateMachine<BattleUIStateMachine, BattleUIStateMachine.BattleUIState, BattleUIStateInput> {
    [SerializeField] public BattleStateMachine _battleStateMachine;
    
    public new delegate void StateTransition(BattleUIState state, BattleUIStateInput input);
    public event StateTransition OnStateTransition ;

    public new delegate void StaminaConsumption(BattleUIStateInput input);
    public event StaminaConsumption OnStaminaConsumption;

    protected override void Start() {
        base.Start();
        _battleStateMachine.OnStateTransition += Refresh;
    }

    protected override void SetInitialState() {
        Transition<BattleUI_Limbo>();
    }

    protected void Refresh(BattleStateMachine.BattleState state, BattleStateInput input) {
        if (state is BattleStateMachine.TurnState && input.ActiveActor() is CharacterActor) {
            CurrInput.AnimationHandler = input.ActiveActor().transform.GetComponent<UIAnimationHandler>();
            CurrInput.actor = (CharacterActor) input.ActiveActor();
            Transition<InitUIState>();
        }
    }

    protected void ExitUI() {
        CurrInput.AnimationHandler = null;
        CurrInput.actor = null;
    }

    /// <summary>
    /// Potato Input:
    /// 0 NORTH
    /// 1 EAST
    /// 2 SOUTH
    /// 3 WEST
    /// 4 NONE
    /// </summary>
    /// <returns></returns>
    private int CheckInput() {
        if (CurrInput.Locked) return 4;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) return 0;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.RightArrow)) return 1;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) return 2;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow)) return 3;
        return 4;
    }

    public void LockUI() {
        CurrInput.Locked = true;
    }

    public void UnlockUI() {
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
