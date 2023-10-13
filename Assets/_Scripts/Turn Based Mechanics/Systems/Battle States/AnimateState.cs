using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class AnimateState : BattleState {

        private Movement _movement = new Movement();    //potato code
        
        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            Debug.Log("Entering animate state");

            // Old Enemy Code, should implement Enemy AI in TargetSelectState instead
            //
            //if (Input.ActiveActor() is CharacterActor) {
            //    Input.ActiveSkill().ActivateSkill();
            //} else if (Input.ActiveActor() is EnemyActor) {    //get rid of this aaaaa
            //    int target = Input.CurrTurn() % 2 == 0 ? 0 : MySM.actorList.Count - 1;
            //    Input.SetActiveSkill(new SkillAction(Input.ActiveActor().data.SkillList()[0], MySM.actorList[target]));
            //    Input.ActiveSkill().ActivateSkill();
            //}
            if (Input.ActiveActor() is EnemyActor) Input.ActiveActor().GetComponentInChildren<Animator>().SetTrigger("_Attack");
            else Input.ActiveActor().GetComponentInChildren<Animator>().SetTrigger("_Attack");
            MySM.OnStateTransition.Invoke(this, Input);

            Input.ActivateSkill();

            //Debug.Log(Input.SkillPrep.targets.Length);
            if (Input.ActiveActor() is CharacterActor) _movement.Bump(Input.ActiveActor().transform, Input.SkillPrep.targets[0].transform); // HARD CODED (change later bc anumation??? idk)

            // here?
            for (int j = 0; j < Input.SkillPrep.targets.Length; j++)
            {
                MySM._eventSequencer.CheckForEvents(Input.SkillPrep.skill.ComputeSkillActionValues(Input.SkillPrep.targets[j]));
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
