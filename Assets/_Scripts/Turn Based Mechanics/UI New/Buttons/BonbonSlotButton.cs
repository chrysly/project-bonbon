using UnityEngine;

namespace BattleUI {
    [RequireComponent(typeof(BonbonBakeSlotButton))]
    public class BonbonSlotButton : BaseSlotButton<BonbonMainHandler> {

        public override BonbonObject Bonbon => StateHandler.Inventory[Slot];

        public override void Activate() {
            base.Activate();
            if (Bonbon == null) {
                StateHandler.Transition<BonbonCraftHandler>(Slot);
            } else StateHandler.Transition<BonbonOptionsHandler>(Slot);
        }
    }
}