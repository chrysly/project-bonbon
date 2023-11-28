namespace BattleUI {
    public class BonbonPassButton : UIButtonBase<BonbonOptionsHandler> {

        public override bool IsAvailable() {
            if (stateHandler.CurrActor.Stamina < stateHandler.ShareStaminaCost) return false;
            Actor target = stateHandler.FetchPassTarget();
            return target != null && target.CanPassBonbon(stateHandler.Slot, target);
        }

        public override void Activate() {
            base.Activate();
            stateHandler.PassBonbon();
        }
    }
}