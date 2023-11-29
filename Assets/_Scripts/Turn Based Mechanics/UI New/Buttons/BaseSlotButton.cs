namespace BattleUI {
    public abstract class BaseSlotButton<T> : UIButtonBase<T> where T : UIStateHandler {
        public abstract int Slot { get; }
        public abstract BonbonObject Bonbon { get; }
    }
}