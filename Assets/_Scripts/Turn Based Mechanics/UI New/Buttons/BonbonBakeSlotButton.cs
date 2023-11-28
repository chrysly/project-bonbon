using UnityEngine;

namespace BattleUI {
    [RequireComponent(typeof(BonbonSlotButton))]
    public class BonbonBakeSlotButton : BaseSlotButton<BonbonBakeHandler> {

        private BonbonBlueprint recipe;

        public void Enable() {
            recipe = stateHandler.Inventory[Slot] == null ? null
                                 : stateHandler.FindValidRecipe(Slot);
        }

        public override bool IsAvailable() {
            return Slot != stateHandler.Slot && recipe != null;
        }

        public override void Activate() {
            base.Activate();
            stateHandler.MatchAndBake(Slot, recipe);
        }
    }
}