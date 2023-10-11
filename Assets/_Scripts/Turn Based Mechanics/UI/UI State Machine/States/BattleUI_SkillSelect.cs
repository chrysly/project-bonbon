using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleUIStateMachine {
    public class BattleUI_SkillSelect : BattleUIStateMachine.BattleUIState {
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
        }

        protected override void RunPostAnimation() {
            base.RunPostAnimation();
        }

        #endregion Animations
    }
}
