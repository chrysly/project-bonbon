using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class AnimateState : BattleState {

        private Movement _movement = new Movement();    //potato code
        
        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            Debug.Log("Entering animate state");

            MySM.OnStateTransition.Invoke(this, Input);

            MySM.GetComponentInParent<AnimationRunner>().OnSkillTrigger(Input.SkillPrep.skill);
            Input.ActivateSkill();

            //Debug.Log(Input.SkillPrep.targets.Length);
            //if (Input.ActiveActor() is CharacterActor) _movement.Bump(Input.ActiveActor().transform, Input.SkillPrep.targets[0].transform); // HARD CODED (change later bc anumation??? idk)

            // here?
            for (int j = 0; j < Input.SkillPrep.targets.Length; j++)
            {
                MySM._eventSequencer.CheckForEvents(Input.SkillPrep.skill.ComputeSkillActionValues(Input.SkillPrep.targets[j], Input.CurrTurn()));
            }

            if (MySM._eventSequencer.RunNextEvent()) {
                MySM.ToggleMachine(true);
                Input.resetSkillPrep();
            }
            else {
                Input.resetSkillPrep();
                MySM.StartBattle(1f);
            }
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
