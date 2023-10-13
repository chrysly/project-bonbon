using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleUIStateMachine {
    public class BattleUI_IngredientSelect : BattleUIStateMachine.BattleUIState {
        public override void Enter(BattleUIStateInput i) {
            base.Enter(i);
            RunPreAnimation();
        }
        
        public override void Update() {
            base.Update();
            int input = MySM.CheckInput();
            if (input == 0 || input == 2) {
                Input.AnimationHandler.ingredientWindow.ButtonSelect(input == 0);
            } else if (input == 1) {
                MySM._battleStateMachine.SwitchToBonbonState(Input.AnimationHandler.ingredientWindow.ConfirmBonbon(),
                    Input.AnimationHandler.ingredientWindow.slot, new bool[4]);
                MySM.DelayedTransition<BattleUI_BonbonMenu>(0.2f, false);
            } else if (input == 3) {
                MySM.DelayedTransition<BattleUI_BonbonMenu>(0.2f, false);
            }
        }

        public override void Exit(BattleUIStateInput i) {
            base.Exit(i);
            RunPostAnimation();
        }
        
        #region Animations

        protected override void RunPreAnimation() {
            base.RunPreAnimation();
            Input.AnimationHandler.ingredientWindow.Initialize(0.5f, Input.actor);
        }

        protected override void RunPostAnimation() {
            base.RunPostAnimation();
            Input.AnimationHandler.ingredientWindow.ToggleMainDisplay(false);
        }

        #endregion Animations
    }
}
