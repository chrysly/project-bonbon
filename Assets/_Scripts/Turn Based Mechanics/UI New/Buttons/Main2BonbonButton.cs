namespace BattleUI {
    public class Main2BonbonButton : UIButtonBase<MainStateHandler> {
        public override void Activate() {
            base.Activate();
            stateHandler.Transition<BonbonMainHandler>();
        }
    }
}