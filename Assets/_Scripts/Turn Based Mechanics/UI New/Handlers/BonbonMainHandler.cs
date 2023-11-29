using System.Linq;
using System.Collections.Generic;

namespace BattleUI {
    public class BonbonMainHandler : UIStateHandler {

        public BonbonObject[] Inventory => Brain.CurrActor.BonbonInventory;

        private void Awake() { Type = UIStateType.Bonbon; }

        public void Transition<T>(int slot) where T : UIStateHandler {
            Brain.Transition<T>(new BonbonTransitionInfo(slot));
        }
    }
}