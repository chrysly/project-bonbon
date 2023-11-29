using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class AnimateState : BattleState {

        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            Debug.Log("Entering animate state");
            
            MySM.OnStateTransition.Invoke(this, Input);
            //OLD --> eve seq code
            //for (int j = 0; j < Input.SkillPrep.targets.Length; j++)
            //{
            //    MySM._eventSequencer.CheckForEvents(Input.SkillPrep.skill.ComputeSkillActionValues(Input.SkillPrep.targets[j]));
            //}
            
            
            //if (MySM._eventSequencer.RunNextEvent()) {
            //    MySM.ToggleMachine(true);
            //}
        }
        
        public override void Update() {
            base.Update();
            //Debug.Log("Running animate state");
        }

        public override void Exit(BattleStateInput input) {
            base.Exit(input);
        }
    }
}
