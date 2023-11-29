namespace BattleUI {
    public class BonbonPassButton : UIButtonBase<BonbonOptionsHandler> {

        public override bool IsAvailable() {
            if (StateHandler.CurrActor.Stamina < StateHandler.ShareStaminaCost) return false;
            Actor target = StateHandler.FetchPassTarget();
            return target != null && StateHandler.CurrActor.CanPassBonbon(StateHandler.Slot, target);
        }

        public override void Activate() {
            base.Activate();
            StateHandler.PassBonbon();
        }
    }
}