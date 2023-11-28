using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public partial class BattleUIStateMachine {
    public class BattleUI_IngredientSelect : BattleUIStateMachine.BattleUIState {
        public override void Enter(BattleUIStateInput i) {
            base.Enter(i);
            RunPreAnimation();
            Debug.Log("IN INGREDIENT SELECT");
        }
        
        public override void Update() {
            base.Update();
            int input = MySM.CheckInput();
            if (input == 0 || input == 2) {
                Input.AnimationHandler.ingredientWindow.ButtonSelect(input == 0);
            } else if (input == 1) {
                if (Input.actor.BonbonInventory[Input.AnimationHandler.ingredientWindow.slot] != null) {
                    MatchRecipe(Input.AnimationHandler.ingredientWindow.slot,
                        Input.AnimationHandler.ingredientWindow.ConfirmBonbon());
                }
                else {
                    if (BattleStateMachine.Instance.CurrInput.ActiveActor().Stamina
                        >= Input.AnimationHandler.ingredientWindow.ConfirmBonbon().craftStamina) {
                        /*MySM.battleStateMachine.SwitchToBonbonState(
                            Input.AnimationHandler.ingredientWindow.ConfirmBonbon(),
                            Input.AnimationHandler.ingredientWindow.slot, new bool[4]);*/
                        MySM.DelayedTransition<BattleUI_BonbonMenu>(0.2f, false);
                    }
                }
            } else if (input == 3) {
                MySM.DelayedTransition<BattleUI_BonbonMenu>(0.2f, false);
            }
        }

        public override void Exit(BattleUIStateInput i) {
            base.Exit(i);
            RunPostAnimation();
        }

        private void MatchRecipe(int slot, BonbonBlueprint bonbon) {
            BonbonHandler factory = MySM.battleStateMachine.CurrInput.BonbonHandler;
            Debug.Log(Input.actor.BonbonInventory[slot].Data.name + " and " + bonbon.name);
            List<BonbonBlueprint> blueprint = factory.FindExactRecipes(Input.actor.BonbonInventory[slot].Data, bonbon);
            if (blueprint != null) {
                Input.actor.BonbonInventory[slot] = blueprint[0].InstantiateBonbon(Input.actor);
            }
            else {
                Debug.Log("not valid recipe");
            }
            MySM.DelayedTransition<BattleUI_BonbonMenu>(0.2f, false);
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
