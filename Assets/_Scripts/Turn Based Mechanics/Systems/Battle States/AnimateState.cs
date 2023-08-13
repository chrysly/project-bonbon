using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class AnimateState : BattleState {
        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            Debug.Log("Entering animate state");
            MySM.StartBattle(3f);
        }
        
        public override void Update() {
            base.Update();
            Debug.Log("Running animate state");
        }
        
        private  IEnumerator ScheduleNextTurn() {
            yield return new WaitForSeconds(3f);
            EnterBattle();
            yield return null;
        }

        public override void Exit(BattleStateInput input) {
            base.Exit(input);
        }
    }
}
