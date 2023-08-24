using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class AnimateState : BattleState {

        private Movement _movement = new Movement();    //potato code
        
        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            Debug.Log("Entering animate state");
            if (Input.ActiveActor() is CharacterActor) {
                Input.ActiveSkill().ActivateSkill();
            } else if (Input.ActiveActor() is EnemyActor) {    //get rid of this aaaaa
                int target = Input.CurrTurn() % 2 == 0 ? 0 : 3;
                Input.SetActiveSkill(new SkillAction(Input.ActiveActor().data.SkillList()[0], MySM.actorList[target]));
                Input.ActiveSkill().ActivateSkill();
            }
            
            _movement.Bump(Input.ActiveActor().transform, Input.ActiveSkill().Target().transform);
            
            MySM.OnStateTransition.Invoke(this, Input);
            MySM.StartBattle(3f);
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
