namespace BattleUI {
    public class UITransitionData {
        public UIStateHandler handler;
        public UIInputPack input;

        public UITransitionData(UIStateHandler handler, UIInputPack input) {
            this.handler = handler;
            this.input = input;
        }
    }
}