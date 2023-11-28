namespace BattleUI {
    public class Main2BonbonButton : UIButtonBase<MainStateHandler> {
        public override void Activate() {
            base.Activate();
            StateHandler.Transition<BonbonMainHandler>();
        }
    }
}