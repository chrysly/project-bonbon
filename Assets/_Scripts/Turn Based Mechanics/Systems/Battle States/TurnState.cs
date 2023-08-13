using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class TurnState : BattleState {
        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            Debug.Log("Entering Turn State");
        }
        
        public override void Update() {
            base.Update();
            Debug.Log("Running Turn State");
        }

        public override void Exit(BattleStateInput input) {
            base.Exit(input);
        }
    }
}
