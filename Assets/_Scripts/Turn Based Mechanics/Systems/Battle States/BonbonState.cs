using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class BonbonState : BattleState {

        private Movement _movement = new Movement();    //potato code
        
        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            Debug.Log("Entering Bonbon state");
            
            _movement.Bump(Input.ActiveActor().transform);  // HARD CODED (change later bc anumation??? idk)
            MySM.OnStateTransition.Invoke(this, Input);
        }
        
        public override void Update() {
            base.Update();
            Debug.Log("Running animate state");
        }

        public override void Exit(BattleStateInput input) {
            base.Exit(input);
        }
    }
}
