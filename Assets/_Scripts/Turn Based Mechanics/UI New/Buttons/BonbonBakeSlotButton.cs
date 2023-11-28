using UnityEngine;

namespace BattleUI {
    [RequireComponent(typeof(BonbonSlotButton))]
    public class BonbonBakeSlotButton : BaseSlotButton<BonbonBakeHandler> {

        private BonbonBlueprint recipe;

        public void Enable() {
            recipe = StateHandler.Inventory[Slot] == null ? null
                                 : StateHandler.FindValidRecipe(Slot);
        }

        public override bool IsAvailable() {
            return Slot != StateHandler.Slot && recipe != null;
        }

        public override void Activate() {
            base.Activate();
            StateHandler.MatchAndBake(Slot, recipe);
        }
    }
}