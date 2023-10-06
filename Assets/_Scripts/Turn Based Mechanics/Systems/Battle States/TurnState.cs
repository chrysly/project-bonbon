using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleStateMachine {
    public class TurnState : BattleState {

        public override void Enter(BattleStateInput i) {
            base.Enter(i);
            MySM.OnStateTransition.Invoke(this, Input);
            Debug.Log("[" + Input.CurrTurn() + "] " + "Entering " + Input.ActiveActor().Data.DisplayName + "'s Turn");
            Input.ActiveActor().TurnStart();
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

            SkillAction skill = Input.ActiveActor().SkillList[0];    //hard coded for pitch demo
            Input.SetSkillPrep(skill);

            Debug.Log(Input.SkillPrep.skill.ToString());
        }
    }
}
