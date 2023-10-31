using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class WinState : BattleState {
        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            MySM.OnStateTransition.Invoke(this, Input);
            Debug.Log($"You won the battle!");
            MySM.winCanvas.gameObject.SetActive(true);
        }
    }
}
