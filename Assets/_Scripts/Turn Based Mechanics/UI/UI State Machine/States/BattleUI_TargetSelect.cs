using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class BattleUIStateMachine {
    public class BattleUI_TargetSelect : BattleUIStateMachine.BattleUIState {
        public override void Enter(BattleUIStateInput i) {
            base.Enter(i);
            RunPreAnimation();
        }
        
        public override void Update() {
            base.Update();
            int input = MySM.CheckInput();
            if (input == 0 || input == 2) {
                Input.AnimationHandler.targetWindow.Select(input != 0);
            } else if (input == 1) {
                MySM._battleStateMachine.ConfirmTargetSelect(Input.AnimationHandler.targetWindow.Confirm());
            } else if (input == 3) {
                MySM.Transition<BattleUI_SkillSelect>();
            }
        }

        public override void Exit(BattleUIStateInput i) {
            base.Exit(i);
            RunPostAnimation();
        }
        
        #region Animations

        protected override void RunPreAnimation() {
            base.RunPreAnimation();
            List<Actor> actors = MySM._battleStateMachine.GetActors();
            actors.Remove(MySM._battleStateMachine.CurrInput.ActiveActor());
            Input.AnimationHandler.targetWindow.Initialize(actors);
        }

        protected override void RunPostAnimation() {
            base.RunPostAnimation();
            Input.AnimationHandler.targetWindow.Disable();
        }

        #endregion Animations
    }
}
