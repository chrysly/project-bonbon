using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class TurnState : BattleState {
        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            MySM.OnStateTransition.Invoke(this, Input);
            Debug.Log("[" + Input.CurrTurn() + "] " + "Entering " + Input.ActiveActor().data.DisplayName() + "'s Turn");
            
            if (Input.ActiveActor() is EnemyActor) {
                MySM.Transition<TargetSelectState>();
            }
        }
        
        public override void Update() {
            base.Update();
            Debug.Log("Running Turn State");
        }

        public override void Exit(BattleStateInput input) {
            base.Exit(input);

            SkillObject skill = Input.ActiveActor().data.SkillList()[0];    //hard coded for pitch demo
            Input.SetActiveSkill(new SkillAction(skill));

            Debug.Log(Input.ActiveSkill().ToString());
        }
    }
}
