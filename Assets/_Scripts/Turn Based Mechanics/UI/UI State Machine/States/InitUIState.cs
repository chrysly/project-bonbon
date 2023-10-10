using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleUIStateMachine {
    public class InitUIState : BattleUIState {
        public override void Enter(BattleUIStateInput i) {
            base.Enter(i);
            RunPreAnimation();
        }
        
        public override void Update() {
            base.Update();
        }

        public override void Exit(BattleUIStateInput i) {
            base.Exit(i);
            RunPostAnimation();
        }
        
        #region Animations

        protected override void RunPreAnimation() {
            base.RunPreAnimation();
            Input.AnimationHandler.ToggleMainPanel(true);
        }

        protected override void RunPostAnimation() {
            base.RunPostAnimation();
            Input.AnimationHandler.ToggleMainPanel(false);
        }

        #endregion Animations
    }
}