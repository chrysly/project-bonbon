using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core;
using UnityEngine;

public partial class BattleStateMachine {
    public class BattleStart : BattleState {

        public override void Enter(BattleStateInput input) {
            Debug.Log("Switched Start State");
        }

        public override void FixedUpdate() {
            base.Update();
        }

        public override void Update() {
            base.Update();
            //Debug.Log("Running Start State");
        }

        public override void Exit(BattleStateInput input) {
            base.Exit(input);
        }
    }
}
