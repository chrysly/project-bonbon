using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class TargetSelectState : BattleState {
        private SelectorManager _selectManager = new SelectorManager();
        //private int _nextSelectedActor = 0; // Enemy Actor Selection

        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            Debug.Log("Entering target select state");
            MySM.OnStateTransition.Invoke(this, Input);
        }

        public override void Update() {
            base.Update();
            Actor actor = _selectManager.CheckForSelect();
            if (actor != null) {
                Input.UpdateSkill(null, new Actor[] { actor });
                MySM.Transition<AnimateState>();
            }
        }

        public override void Exit(BattleStateInput input) {
            base.Exit(input);
        }
    }
}