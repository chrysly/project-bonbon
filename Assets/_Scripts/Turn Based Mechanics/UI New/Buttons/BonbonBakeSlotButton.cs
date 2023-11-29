using UnityEngine;

namespace BattleUI {
    [RequireComponent(typeof(BonbonSlotButton))]
    public class BonbonBakeSlotButton : BaseSlotButton<BonbonBakeHandler> {

        public override BonbonObject Bonbon => StateHandler.Inventory[Slot];
        private BonbonBlueprint recipe;

        public void Enable() {
            recipe = Bonbon == null ? null
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