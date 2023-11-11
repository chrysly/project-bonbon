using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class AnimateState : BattleState {

        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            Debug.Log("Entering animate state");
            
            if (Input.SkillPrep.bonbon != null) Debug.Log("Bonbon Name: " + Input.SkillPrep.bonbon.Data.name);
            else Debug.Log("NO BONBON AAAAAAAAAAA");
            
            MySM.OnStateTransition.Invoke(this, Input);

            var res = Input.ActivateSkill();
            if (res != null) Input.AnimateSkill(res.skill, res.bonbon);

            for (int j = 0; j < Input.SkillPrep.targets.Length; j++)
            {
                MySM._eventSequencer.CheckForEvents(Input.SkillPrep.skill.ComputeSkillActionValues(Input.SkillPrep.targets[j], Input.CurrTurn()));
            }
            
            
            if (MySM._eventSequencer.RunNextEvent()) {
                MySM.ToggleMachine(true);
            }
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
