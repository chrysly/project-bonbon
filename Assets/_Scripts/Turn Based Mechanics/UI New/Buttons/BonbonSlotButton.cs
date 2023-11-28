using UnityEngine;

namespace BattleUI {
    [RequireComponent(typeof(BonbonBakeSlotButton))]
    public class BonbonSlotButton : BaseSlotButton<BonbonMainHandler> {

        private BonbonObject Bonbon => stateHandler.Inventory[Slot];

        public override void Activate() {
            base.Activate();
            if (Bonbon == null) {
                stateHandler.Transition<BonbonCraftHandler>(Slot);
            } else stateHandler.Transition<BonbonOptionsHandler>(Slot);
        }
    }
}