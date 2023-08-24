using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class TargetSelectState : BattleState {
        private SelectorManager _selectManager = new SelectorManager();
        
        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            Debug.Log("Entering target select state");
            if (Input.ActiveActor() is EnemyActor) {
                MySM.Transition<AnimateState>();
            }
            MySM.OnStateTransition.Invoke(this, Input);
        }
        
        public override void Update() {
            base.Update();
            Actor actor = _selectManager.CheckForSelect();
            if (actor != null) {
                Input.SetActiveSkill(new SkillAction(Input.ActiveSkill().Data(), actor));
                MySM.Transition<AnimateState>();
            }
        }

        public override void Exit(BattleStateInput input) {
            base.Exit(input);
        }
    }
}
