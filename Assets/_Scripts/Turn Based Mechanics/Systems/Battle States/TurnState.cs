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
            if (!Input.ActiveActor().Available) Input.AdvanceTurn();

            if (Input.ActiveActor() is EnemyActor) {
                // Enemy Actor Skill Selection
                ActiveSkillPrep skillPrep = EnemyAI.ChooseEnemyAISkill(Input.ActiveActor(), Input.TurnQueue);
                Input.UpdateSkill(skillPrep.skill, skillPrep.targets);
                MySM.Transition<AnimateState>();
            } Input.Initialize();
        }
        
        public override void Update() {
            base.Update();
            //Debug.Log("Running Turn State");
        }

        public override void Exit(BattleStateInput input) {
            base.Exit(input);
        }
    }
}
