using System.Linq;
using System.Collections.Generic;

namespace BattleUI {
    public class BonbonMainHandler : UIStateHandler {

        public BonbonObject[] Inventory => Brain.CurrActor.BonbonInventory;

        private void Awake() { Type = UIStateType.Bonbon; }

        public override void Init(UIBrain brain) {
            base.Init(brain);
            IEnumerable<BonbonSlotButton> slots = buttonMatrix.Values.Select(button => button as BonbonSlotButton);
            for (int i = 0; i < slots.Count(); i++) slots.ElementAt(i).Init(i);
        }

        public void Transition<T>(int slot) where T : UIStateHandler {
            Brain.Transition<T>(new BonbonTransitionInfo(slot));
        }
    }
}