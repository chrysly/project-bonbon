namespace BattleUI {
    public class BonbonConsumeButton : UIButtonBase<BonbonOptionsHandler> {

        public override void Activate() {
            base.Activate();
            StateHandler.ConsumeBonbon();
        }
    }
}