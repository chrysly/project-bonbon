namespace BattleUI {
    public class EndTurnButton : UIButtonBase<MainStateHandler> {

        public override void Activate() {
            base.Activate();
            stateHandler.Brain.ExitUI();
        }
    }
}