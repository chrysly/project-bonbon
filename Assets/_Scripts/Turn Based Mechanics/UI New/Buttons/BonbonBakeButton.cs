namespace BattleUI {
    public class BonbonBakeButton : UIButtonBase<BonbonOptionsHandler> {

        public override void Activate() {
            base.Activate();
            StateHandler.Transition<BonbonBakeHandler>();
        }
    }
}