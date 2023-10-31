using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class LoseState : BattleState {
        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            MySM.OnStateTransition.Invoke(this, Input);
            Debug.Log($"You lost the battle...");
            MySM.loseCanvas.gameObject.SetActive(true);
        }
    }
}
